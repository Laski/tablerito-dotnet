#!/bin/sh
# Deploy to production
git pull
dotnet publish Tablerito -o publish
cd publish && dotnet Tablerito.dll
