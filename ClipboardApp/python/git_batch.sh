#!/bin/bash

GIT_PATH=$(readlink -f $1)
BRANCH=$2

BASEDIR=$(readlink -f $(dirname $0))

function update() {
        echo "update"

        python3 ${BASEDIR}/git_file_processor.py update $1
}
function delete() {
        echo "delete"
        python3 ${BASEDIR}/git_file_processor.py delete $1
}

# if branch is not specified, use the current branch
if [ -z $BRANCH ]; then
        BRANCH=$(git symbolic-ref --short HEAD)
fi

git checkout $BRANCH
git pull origin $BRANCH

git diff --name-status origin/$BRANCH..HEAD | while read line
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
