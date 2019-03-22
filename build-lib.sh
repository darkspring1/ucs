#!/bin/bash

ASSEMBLY_VERSION=$CORE_MAJOR_VERSION.$CORE_MINOR_VERSION.$CORE_RELEASE_VERSION.$CI_PIPELINE_ID

SERVICE_NAME="office-back"

function replaceInFile()
{
    local ORIGINAL=$1
    local NEW=$2
    local FILE=$3
    
    #echo "ORIGINAL"
    #echo $ORIGINAL
    #echo "NEW"
    #echo $NEW
    #echo "FILE"
    #echo $FILE
    
    sudo sed -i "s/$ORIGINAL/$NEW/g" $FILE
}

function replaceSecretVariables
{
    local ENV=$1
    local SETTING_FILE=$2
    local VAR_POSTFIX=$3
    local VAR_NAME=$ENV'_'$VAR_POSTFIX
    local VAR_VALUE=${!VAR_NAME}
    replaceInFile $VAR_NAME "$VAR_VALUE" $SETTING_FILE
    echo "SECRET VAR '$VAR_NAME' WAS SET"
}

function SetAssemblyVersion()
{
    local PROJ_FILE=$1
    local ASSEMBLY_VERSION_TAGS="<AssemblyVersion>$ASSEMBLY_VERSION<\/AssemblyVersion>"
    replaceInFile "<AssemblyVersion>.*<\/AssemblyVersion>" $ASSEMBLY_VERSION_TAGS $PROJ_FILE
}

#устанавливаем пароли и прочую секретную информацию
function SetSecretVariables
{
    local ENV=${1^^}
    local SETTING_FILE=$2
    
    #установим пароль от Rabbit
    replaceSecretVariables $ENV $SETTING_FILE 'RABBIT_PASSWORD'
    #установим ключ для шифрования jwt-токенов
    replaceSecretVariables $ENV $SETTING_FILE 'JWT_KEY'
    #токен для авторизации в лицензировании
    replaceSecretVariables $ENV $SETTING_FILE 'LICENSE_TOKEN'
	#установим RECAPTCHA_SECRET_V3
    replaceSecretVariables $ENV $SETTING_FILE 'RECAPTCHA_SECRET_V3'
    #установим пароль от pg
    replaceSecretVariables $ENV $SETTING_FILE 'PG_PASSWORD'
}

function patchDb
{
    # ждём пока ядро стартанёт
    echo 'please wait 20s...'
    sleep 20s
    local ENV=$1
    local TOKEN="Authorization: $2"
    local HTTP_CODE=$(sudo curl -s -o /dev/null -I -w "%{http_code}" -H "Content-Type: application/json" -H "$TOKEN" -X PATCH http://$ENV-rk-lite.ucs.ru/api/admin/clients/client1?forceUpdate=true&updateTest=true;)
    if [[ $HTTP_CODE -eq 200 ]]; then
        exit 0
    fi
    exit -1
}

function test()
{
    local ENV=$1
    local FILTER=$2
    local SOLUTION_DIR='rk_lite/RkLiteCore/'
    local IMAGE_NAME=rk_lite_core/tests/$ENV
    cd $SOLUTION_DIR
    sudo docker build --build-arg FILTER=$FILTER -t $IMAGE_NAME -f Test.Dockerfile .
    sudo docker rmi $IMAGE_NAME
}

function build()
{
    local PROJ_NAME='Rklc.WebApi'
    local ENV=$1
    local SOLUTION_DIR='rk_lite/RkLiteCore/'
    local PROJ_DIR=$SOLUTION_DIR"$PROJ_NAME/"
    local STARTED_PROJ_FILE=$PROJ_DIR"$PROJ_NAME.csproj"
    #local SETTING_FILE=$PROJ_DIR"appsettings.$ENV.json"

    local IMAGE_NAME=$DOCKER_REGISTRY_URL/rk_lite/rk_lite_core/$ENV

    SetAssemblyVersion $STARTED_PROJ_FILE
    #SetSecretVariables $ENV $SETTING_FILE

    cd $SOLUTION_DIR
    sudo docker build --build-arg PROJ_NAME=$PROJ_NAME -t $IMAGE_NAME:$ASSEMBLY_VERSION .
    sudo docker login -u gitlab-ci-token -p $CI_JOB_TOKEN $CI_REGISTRY
    sudo docker push $IMAGE_NAME
    sudo docker rmi $IMAGE_NAME:$ASSEMBLY_VERSION
}

function deploy()
{
    local ENV=$1
    local UPPER_ENV=${ENV^^}
    local IMAGE_NAME=$DOCKER_REGISTRY_URL/rk_lite/rk_lite_core/$ENV:$ASSEMBLY_VERSION
    
    local KUBE_CONFIG_VAR_NAME=$UPPER_ENV'_KUBE_CONFIG'
    local KUBE_CONFIG_VAR_VAL=${!KUBE_CONFIG_VAR_NAME}
    local KUBE_DIR="$PWD/kube"
    
    mkdir -p -m 777 $KUBE_DIR
    echo "$KUBE_CONFIG_VAR_VAL" > $KUBE_DIR/config
    chmod 777 $KUBE_DIR/config

    sudo docker run --rm --net=host -v $KUBE_DIR:/config/.kube wernight/kubectl set image deployment/$SERVICE_NAME $SERVICE_NAME=$IMAGE_NAME -n=$ENV || {
        #чистим за собой
        sudo rm -r kube
        #Exit with failure
        exit -1
    }
    #чистим за собой
    sudo rm -r kube
}