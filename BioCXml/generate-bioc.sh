#!/usr/bin/env bash

dotnet tool run xscgen \
    --netCore \
    --nullable \
    --namespace="BioCXml" \
    --collectionType="System.Collections.Generic.List\`1" \
    --collectionSettersMode=Init \
    --complexTypesForCollections=False \
    --compactTypeNames \
    --nullableReferenceAttributes \
    --separateFiles \
    --interface=False \
    --codeTypeReferenceOptions=GenericTypeParameter \
    -o ..\
    -v \
    BioC.xsd
