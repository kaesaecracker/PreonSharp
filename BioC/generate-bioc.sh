#!/usr/bin/env bash

dotnet tool run xscgen \
    --netCore \
    --nullable \
    --namespace="BioC" \
    --collectionType="System.Collections.Generic.List\`1" \
    --complexTypesForCollections=false \
    --compactTypeNames \
    --nullableReferenceAttributes \
    --separateFiles \
    --unionCommonType \
    -o ..\
    BioC.xsd
