@echo off

IF "%1" == "r" (SET cla="/p:Configuration=Release")
IF "%1" == "rc" (SET cla="/p:Configuration=Release" & SET clb="/t:Rebuild")
IF "%1" == "c" (SET cla="/t:Rebuild")

pushd graphicsscript
echo === Compiling Facer.GraphicsScript
msbuild /nologo /v:q %cla% %clb% %2 facer.graphics.script.csproj
popd

pushd script
echo === Compiling Facer.Script
msbuild /nologo /v:q %cla% %clb% %2 facer.script.csproj
popd

pushd bootstrapscript
echo === Compiling Facer.BootstrapScript
msbuild /nologo /v:q %cla% %clb% %2 facer.bootstrap.script.csproj
popd

