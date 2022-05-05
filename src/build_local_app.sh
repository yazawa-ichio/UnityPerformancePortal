
dotnet publish UnityPerformancePortal.Front -c Release -o obj/front
rm -rf UnityPerformancePortal.Server/wwwroot
cp -r obj/front/wwwroot UnityPerformancePortal.Server/wwwroot
dotnet publish UnityPerformancePortal.Server --self-contained true -c Release -o obj/upp-win --os win -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true
dotnet publish UnityPerformancePortal.Server --self-contained true -c Release -o obj/upp-linux --os linux -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true
dotnet publish UnityPerformancePortal.Server --self-contained true -c Release -o obj/upp-osx --os osx -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true
