properties { 
  $base_dir  = resolve-path .
  $lib_dir = "$base_dir\lib"
  $build_dir = "$base_dir\build" 
  $buildartifacts_dir = "$build_dir\" 
  $sln_file = "$base_dir\Ditto.sln" 
  $version = "0.5.0.0"
  $tools_dir = "$base_dir\lib\tools"
  $release_dir = "$base_dir\Release"
  $config = "Debug"
  $target_framework_version = "4.0"
}

$framework = '4.0'

include .\psake_ext.ps1
	
task default -depends Release

task Clean { 
  remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 
	Generate-Assembly-Info `
		-file "$base_dir\src\CommonAssemblyInfo.cs" `
		-title "Ditto $version" `
		-description "Object-Object mapper for .NET" `
		-company "Mike Nichols" `
		-product "Ditto $version" `
		-version $version `
		-copyright "Mike Nichols 2011"
		
	new-item $release_dir -itemType directory 
	new-item $buildartifacts_dir -itemType directory 
} 

task Compile -depends Init {
  $build_properties = "OutDir=$buildartifacts_dir;Configuration=$config"
  if($target_framework_version -eq '4.0')
  {
    $build_properties = "$build_properties;TargetFrameworkVersion=$target_framework_version"
  }
  msbuild $sln_file /p:$build_properties
} 

task Test -depends Compile {
  $test_runner =  "$tools_dir\xUnit\"
  $old = pwd
  cd $build_dir
  if($target_framework_version -eq '4.0')
  {
    & $tools_dir\xUnit\xunit.console.clr4.exe "$build_dir\Ditto.Tests.dll"
  }
  else
  {
    & $tools_dir\xUnit\xunit.console.exe "$build_dir\Ditto.Tests.dll"
  }
  cd $old		
}


task Release  -depends Test{

	& $tools_dir\zip.exe -9 -A -j `
		$release_dir\Ditto.zip `
		$build_dir\Ditto.dll `
    	$build_dir\Ditto.xml `
		$build_dir\Ditto.WindsorIntegration.dll `
    	$build_dir\Ditto.WindsorIntegration.xml `
		$build_dir\Castle.Core.dll `
    	$build_dir\Castle.Core.xml `
		$build_dir\Castle.Facilities.Logging.dll `
    	$build_dir\Castle.Facilities.Logging.xml `
    	$build_dir\Castle.Services.Logging.Log4netIntegration.dll `
    	$build_dir\Castle.Services.Logging.Log4netIntegration.xml `
    	$build_dir\Castle.Windsor.dll `
    	$build_dir\Castle.Windsor.xml `
		$build_dir\Fasterflect.dll `
		$build_dir\Fasterflect.xml `
		$build_dir\log4net.dll `
		$build_dir\log4net.xml `
		$base_dir\license.txt
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
}
