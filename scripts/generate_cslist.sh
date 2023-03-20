#!/bin/bash

files=$(grep -o '<Compile Include="[^"]*"' JointCorrectEE.csproj | sed 's/<Compile Include="//; s/"//')
echo "$files" > JointCorrectEE.cslist
