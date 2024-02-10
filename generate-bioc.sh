#!/usr/bin/env bash

dotnet tool run xscgen \
    --netCore \
    --nullable \
    --namespace="PreonUsage.BioC" \
    --collectionType="System.Collections.Generic.List\`1" \
    --complexTypesForCollections=false \
    --compactTypeNames \
    --nullableReferenceAttributes \
    --separateFiles \
    --unionCommonType \
    --namespaceHierarchy BioC.xsd
