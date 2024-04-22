#!/bin/bash

# オプションを解析
while getopts 'p' flag; do
  case "${flag}" in
    p) 
        propset=1
        ;;
    *) 
        echo "Unknown option" >&2 
        ;;
  esac
done

shift $((OPTIND - 1))

SVN_PATH=$(readlink -f $1)
REVISION=$2

BASEDIR=$(readlink -f $(dirname $0))

login_args=" --username ${SVN_USERNAME} --password ${SVN_PASSWORD} --non-interactive"

if [ -z "$SVN_USERNAME" ]; then
        echo "SVN_USERNAME is not set"
        exit 1
fi
if [ -z "$SVN_PASSWORD" ]; then
        echo "SVN_PASSWORD is not set"
        exit 1
fi

function update() {
    echo "update"

    description=$(svn propget description $1 2>/dev/null)

    python3 ${BASEDIR}/svn_file_processor.py update $1 "${description}"
    if [ -n "$propset" ]; then
        propset_summary $1
    fi
}
function delete() {
    echo "delete"
    python3 ${BASEDIR}/svn_file_processor.py delete $1
}
function propset_summary() {
    echo "propset_summary"
    description=$(svn propget description $1 2>/dev/null)
    if [ -n "$description" ]; then
        echo "description already exists"
        return
    fi
    text=$(python3 ${BASEDIR}/svn_file_processor.py summary $1)
    echo "svn propset description $text $1"
    svn propset description "${text}" $1

}

# if revision is not specified, get latest revision
if [ -z $REVISION ]; then
    # get last processed revision
    if [ -f "last_revision" ]; then
        REVISION=`cat last_revision`
    else
        REVISION=0
    fi
fi

svn update ${SVN_PATH}
echo "svn diff -r ${REVISION}:HEAD --summarize ${SVN_PATH} "
svn diff -r ${REVISION}:HEAD --summarize ${SVN_PATH} 

svn diff -r ${REVISION}:HEAD --summarize ${SVN_PATH} | while read line
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

changed_file_count=$(svn status ${SVN_PATH} | wc -l)
if [ ${changed_file_count} -gt 0 ]; then
    echo "commit propset"
    svn ${login_args} commit -m "update description" ${SVN_PATH}
fi

# update last processed revision
echo $(svn info --show-item revision ${SVN_PATH} ) > last_revision

