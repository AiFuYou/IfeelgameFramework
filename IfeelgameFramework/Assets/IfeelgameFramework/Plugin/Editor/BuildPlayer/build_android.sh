echo build_android.sh

projPath=$1
gradlePath=$2
buildMode=assemblRelease
if [[ $3 == debug ]]; then
	buildMode=assemblDebug
fi

echo projPath:$projPath
echo gradlePath:$gradlePath
echo buildMode:$buildMode

cd $projPath

gradleCMD="java -classpath $gradlePath org.gradle.launcher.GradleMain -Dorg.gradle.jvmargs=-Xmx4096m"

echo clean
$gradleCMD clean

echo packageApk
$gradleCMD $buildMode

echo generate denpendencies
$gradleCMD dependencies > denpendency.txt
