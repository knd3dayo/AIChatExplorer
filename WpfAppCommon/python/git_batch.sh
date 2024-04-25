#!/bin/bash

GIT_PATH=$(readlink -f $1)
BRANCH=$2

BASEDIR=$(readlink -f $(dirname $0))

function update() {
        echo "update"
        PYTHONPATH=${BASEDIR}
        python3 file_processor.py update $1
}
function delete() {
        echo "delete"
        PYTHONPATH=${BASEDIR}
        python3 file_processor.py delete $1
}

# if branch is not specified, use the current branch
if [ -z $BRANCH ]; then
        BRANCH=$(git symbolic-ref --short HEAD)
fi
# 作業ディレクトリに移動
cd $GIT_PATH

git checkout $BRANCH
# 現在の作業ディレクトリのリビジョン
local_rev=$(git rev-parse HEAD)

git pull origin $BRANCH

git diff --name-status ${local_rev}..HEAD | while read line
do
        echo $line
        action=`echo $line | awk '{print $1}'`
        file=`echo $line | awk '{print $2}'`
        case $action in
                "A")
                        update ${file}
                        ;;
                "M")
                        update ${file}
                        ;;
                "D")
                        delete ${file}
                        ;;
                *)
                        echo "unknown action"
        esac
done
