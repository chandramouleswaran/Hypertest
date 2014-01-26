msbuild Hypertest.sln /t:clean
FOR /D %%p IN ("Build\*.*") DO rmdir "%%p" /s /q
rmdir Build /s /q
msbuild Hypertest.sln /p:Configuration=Release
xcopy "Libs"  "Build\External" /E /Y /I /R
xcopy "Driver"  "Build\Driver" /E /Y /I /R