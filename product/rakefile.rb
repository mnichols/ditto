require File.expand_path(File.join(__FILE__),'rake_helper.rb')
require File.join(File.dirname(__FILE__),'cfg.rb')

namespace :ditto do
	desc 'Builds and packages product'
	task :default => [:package]

	desc 'Initializes build'
	task :init => [:environment,:clean,:generate_assembly_info]
	
	desc 'Sets environment properties'
	task :environment do
		@root_dir = File.expand_path(File.join(File.dirname(__FILE__),'..'))		
	end

	desc 'generates assembly info'
	task :generate_assembly_info => [:environment] do
		asm = AssemblyInfoBuilder.new({
			:AssemblyCompany				=> "Mike Nichols",
			:AssemblyCopyright 				=> "Copyright 2011 Mike Nichols",
			:AssemblyDescription			=> 0.1.0,
			:AssemblyFileVersion 			=> configatron.product.file_version,
			:AssemblyInformationalVersion 	=> configatron.product.version,
			:AssemblyProduct				=> "#{configatron.product.name} #{configatron.build.git_tag}",
			:AssemblyTitle 					=> "",
			:AssemblyTrademark 				=> configatron.build.commit,
			:AssemblyVersion 				=> configatron.product.version
		})
			
		asm.write 'CommonAssemblyInfo.cs'.in(configatron.dir.src)
	end

	desc 'Cleans working directories'
	task :clean => [:environment] do
		puts 'cleaning directories'
		
		rm_rf(configatron.dir.build)			
		rm_rf(File.join(configatron.dir.dist,configatron.product.dir)) 
		Dir.glob(File.join(configatron.dir.dist,"#{configatron.product.dir}-*.zip")).each do |file| 
			rm_f file #delete product distribution zip file
		end
		
		MSBuild.compile(
			:project => 'Ditto.sln',
			:clrversion => 'v4.0.30319',
			:target => 'Clean',
			:properties => {
				:Configuration => 'Release'
			}
		)
	end
	

	desc 'Tests the product'
	task :test => [:init] do
		runner = Testrunner.new()
		runner.run
	end

	desc 'Packages product'
	task :package => [:test] do
		package = Package.new()
		package.server	
	end
	
end

def configure_dir(root_dir)
	raise ArgumentError,"root_dir is required" if root_dir.nil?
	dirs = { 
		:root => ".",
		:build => "build",
		:dist => "dist",
		:db => "db",
		:config => "config",
		:deploy => "deploy",
		:lib => "lib",
		:tools => "lib/tools",
		:src => "src",
		:app => "src/app",
		:test => "src/test"
	}
	
	Dir.chdir File.expand_path(root_dir) do			
		exp = dirs.inject({}){ |expanded,(k,v)|expanded[k] = File.expand_path(v); expanded }
		return exp			
	end		
end	

def configure_build(root_dir)
	raise ArgumentError,"root_dir is required" if root_dir.nil?
	
	team_city = ENV["TEAMCITY_VERSION"]		
	version_base = "0.0.0"
	git_tag = nil
	git_number = nil
	commit = nil
	build_number = nil
	
	if(team_city) then
		begin 
			puts 'fetching tags for build'
			`git fetch --tags` # this is because TeamCity does a fetch --no-tags
		rescue 
			puts 'Tags were not fetched!'
		end
	end
	
	begin
		version_base = `git describe --abbrev=0`.chomp.tr('v','')	#relies on having a git tag ie 'v0.3'
		version_base = '0.0.0' if version_base.length == 0
		puts "Base version identified as '#{version_base}'"
		git_tag = `git describe --long`.chomp 	# looks something like v0.1.0-63-g92228f4
		gitnumberpart = /-(\d+)-/.match(git_tag)
		git_number = gitnumberpart.nil? ? '0' : gitnumberpart[1]
		commit = (ENV["BUILD_VCS_NUMBER"].nil? ? `git log -1 --pretty=format:%H` : ENV["BUILD_VCS_NUMBER"])
		
	rescue
		version_base = "0.0.0"
		commit = "git unavailable"
		git_number = '0'
		puts "Unable to determine base version...defaulting"
	end				
	
	build = {
		:team_city 		=> team_city || false,
		:version_base 	=> version_base,
		:config 		=> ENV["config"] || "Release",	
		:tests 			=> ENV["run_tests"] && ENV["run_tests"]=="true",	
		:deploy 		=> ENV["deploy"] && ENV["deploy"]=="true",
		:fresh 			=> ENV["fresh"] || "false",
		:key_file 		=> File.expand_path(File.join(root_dir,"cei.snk")),		
		:commit 		=> commit || '0',
		:git_tag 		=> git_tag || 'v0.0.0',
		:git_number 	=> git_number || '0',
		:number			=> "#{version_base}.#{git_number}"
	}
	
	return build
end

def configure_product(build,convention_dir,src_app_dir)
	raise ArgumentError, 'build must be configured first' if (build.nil? || build.empty?)
	product = {
		:app_name 		=> @app_name,
		:company_name 	=> "Consultant Engineering, Inc",	
		:dir 			=> convention_dir,
		:file_version 	=> "#{build[:version_base]}.#{build[:git_number]}",
		:name 			=> "Materials Management",			
		:package_number => build[:team_city] ? "#{build[:number]}-#{ENV['BUILD_NUMBER']}" : build[:number],
		:src_dir 		=> src_app_dir,
		:version 		=> build[:version_base]			
	}
	return product		
end

def configure_tools(tools_dir,dist_dir)
	raise ArgumentError, 'directories must be configured first', caller if tools_dir.nil?
	tools = {				
		:powershell => File.join('C:/Windows/system32/WindowsPowerShell/v1.0/powershell.exe'),
		:xunit 		=> File.join(tools_dir,'xunit','xunit.console.clr4.exe')		
	}
end