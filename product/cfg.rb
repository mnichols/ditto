require 'configatron'
class Cfg	
	def initialize params		
		@root_dir = File.expand_path(File.join(File.dirname(__FILE__),'..'))				
		
		clr_map = {
			:net4 => 'v4.0.30319',
			:net35 => 'v3.5'
		}		
		app = {
			:app_name		=> 'Ditto',
			:src_app_dir	=> 'Ditto.sln'
		}
		
		@app_name = app[:app_name] #'Materials_Domain'			
		@convention_dir = app[:convention_dir] #'server'
		@src_app_dir = app[:src_app_dir] 
		@clr = clr_map[params[:clr] || :net4] #net4 default
		@sln = File.expand_path(File.join(@root_dir,params[:sln])) #Cei.MaterialsTesting-Server.sln
		
		puts "Configuring #{@app_name} initialized with root directory as '#{@root_dir}', convention directory '#{@convention_dir}' and src_app_dir '#{@src_app_dir}'"
	end
		
	def load
		configatron.reset!		
		
		configatron.clr = @clr
		configatron.sln = @sln				
		
		dir = configure_dir(@root_dir)				
		
		build = configure_build(@root_dir)
		puts "Marking '#{@app_name}' build as '#{build[:number]}' and is_teamcity ='#{build[:team_city]}'"
		
		product = configure_product(build,@convention_dir,@src_app_dir)	
		puts "Using convention '#{product[:dir]}'"
		environment = configure_environment_settings(ENV['env'] || 'dev') #default is dev
		puts "In environment '#{environment[:name]}'"
		
		test = configure_test_settings(build[:team_city])		
		puts "Test environment is '#{ test[:environment] }'"
		
		deploy = configure_deploy_settings(@convention_dir,@src_app_dir,build[:fresh],environment[:name],product,(ENV['clone']||false))		
		
		tools = configure_tools(dir[:tools],dir[:dist])	

		configatron.dir.configure_from_hash(dir)
		configatron.build.configure_from_hash(build)
		configatron.product.configure_from_hash(product)
		configatron.env.configure_from_hash(environment)
		configatron.test.configure_from_hash(test)
		configatron.deploy.configure_from_hash(deploy)
		configatron.tools.configure_from_hash(tools)
	end
	
	private 	
	
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
	
	def configure_environment_settings(environment_name='dev')
		
		
		settings = Hash.new
		
		settings['prod'] = {	
			:cdn_provider					=> '', # web only. change when implementing a static files provider
			:service_name					=>	@app_name,
			:domain_connection_string		=> "Data Source=.\\SQLEXPRESS;Initial Catalog=#{apps[:domain_server][:app_name]};Integrated Security=SSPI",
			:web_connection_string 			=> "mongodb://localhost/#{apps[:web_server][:app_name]}?pooling=true",
			:reports_connection_string 		=> "mongodb://localhost/#{apps[:reports_server][:app_name]}?pooling=true",			
			:web_security_connection_string	=> "mongodb://localhost/#{apps[:web_app][:app_name]}?pooling=true",
			:log_level 						=> 'INFO',
			:name 							=> 'prod',
			:server_endpoint 				=> "rhino.queues://localhost:13000/#{apps[:domain_server][:app_name]}",
			:web_server_endpoint			=> "rhino.queues://localhost:13005/#{apps[:web_server][:app_name]}",
			:reports_server_endpoint		=> "rhino.queues://localhost:13010/#{apps[:reports_server][:app_name]}",
			:require_rebuild				=> "#{ENV['rebuild'] || 'False'}",
			:corp_id		=> "75263FC9-B740-4F41-BFC9-DDEB567EB0FF" # MaterialsTestingCorporationId
		}
		
		settings['dev'] = settings['prod'].dup.merge!({			
			:domain_connection_string		=> "Data Source=.;Initial Catalog=#{apps[:domain_server][:app_name]};Integrated Security=SSPI",
			:service_name					=>	@app_name,			
			:log_level 						=> 'DEBUG',
			:name 							=> 'dev'
		})		
		
		settings['stage'] = settings['prod'].dup.merge!({						
			:service_name					=>	"#{@app_name}_#{environment_name}",
			:domain_connection_string		=> "Data Source=.\\SQLEXPRESS;Initial Catalog=#{apps[:domain_server][:app_name]}_#{environment_name};Integrated Security=SSPI",
			:web_connection_string 			=> "mongodb://localhost/#{apps[:web_server][:app_name]}_#{environment_name}?pooling=true",
			:reports_connection_string 		=> "mongodb://localhost/#{apps[:reports_server][:app_name]}_#{environment_name}?pooling=true",			
			:web_security_connection_string	=> "mongodb://localhost/#{apps[:web_app][:app_name]}_#{environment_name}?pooling=true",
			:log_level 						=> 'DEBUG',
			:name 							=> 'stage',
			:server_endpoint 				=> "rhino.queues://localhost:14000/#{apps[:domain_server][:app_name]}_#{environment_name}",
			:web_server_endpoint			=> "rhino.queues://localhost:14005/#{apps[:web_server][:app_name]}_#{environment_name}",
			:reports_server_endpoint		=> "rhino.queues://localhost:14010/#{apps[:reports_server][:app_name]}_#{environment_name}",
			:require_rebuild				=> "#{ENV['rebuild'] || 'True'}"
		})		
		
		return settings[environment_name]
	end
	
	def configure_test_settings(is_teamcity)		
		apps = get_app_map
		test_settings = {
			:service_name					=>	@app_name,
			:domain_connection_string		=> "Data Source=.;Initial Catalog=#{apps[:domain_server][:app_name]};Integrated Security=SSPI",
			:web_connection_string 			=> "mongodb://localhost/#{apps[:web_server][:app_name]}?pooling=true",
			:reports_connection_string 		=> "mongodb://localhost/#{apps[:reports_server][:app_name]}?pooling=true",			
			:web_security_connection_string	=> "mongodb://localhost/#{apps[:web_app][:app_name]}?pooling=true",
			:log_level 						=> 'DEBUG',			
			:environment					=> 'test',
			:server_endpoint 				=> "rhino.queues://localhost:13000/#{apps[:domain_server][:app_name]}",
			:web_server_endpoint			=> "rhino.queues://localhost:13005/#{apps[:web_server][:app_name]}",
			:reports_server_endpoint		=> "rhino.queues://localhost:13010/#{apps[:reports_server][:app_name]}",
			:require_rebuild				=> "#{ENV['rebuild'] || 'False'}"
		}
		
		ci_settings = test_settings.dup.merge!({			
			:service_name		=>	@app_name,			
			:log_level 			=> 'DEBUG',			
			:environment		=> 'ci'
		})
		
		return is_teamcity ? ci_settings : test_settings
		
	end
	
	def configure_deploy_settings(convention_dir,src_app_dir,is_clean=false,environment_name='dev',product=nil,clone=false)
		raise ArgumentError, "convention_dir is required" if (convention_dir.nil?  || convention_dir.empty?)
		raise ArgumentError, "src_app_dir is required" if (src_app_dir.nil?  || src_app_dir.empty?)
		raise ArgumentError, "product is required" if (product.nil?  || product.empty?)
		
		settings = Hash.new
		settings['prod'] = {
			:environment_name	=> 'prod',
			:key 				=> '(3,4,2,3,56,34,254,222,1,1,2,23,42,54,33,233,1,34,2,8,6,5,35,43)', #TODO : Move this key to a provisioning process on the destination server		
			:service_path 		=> "c:/apps/materialstesting/#{convention_dir}",
			:staging_path		=> "c:/deployment/#{convention_dir}",
			:url 				=> 'https://mgt.qt-corp.com:8172/msdeploy.axd',				
			:service_dll		=>  "#{ src_app_dir }.dll",
			:service_name		=> @app_name,
			:db_name			=> @app_name,
			:server_name		=> '.\SQLEXPRESS',
			:drop_db			=> is_clean,
			:is_clean			=> is_clean,
			:backup_path		=> "c:/data/backups/#{@app_name}_#{product[:file_version]}.bak",
			:data_source		=> nil, #database name from which to clone data
			:db_to_backup		=> @app_name,
			:virtual_dir		=> nil,
			:version			=> "#{product[:version]}",
			:clone_db			=> clone
		}
				
		settings['dev'] = settings['prod'].dup.merge!({
			:environment_name	=> 'dev',
			:server_name		=> '.',
			:url 				=> 'https://localhost:8172/msdeploy.axd'
		})
		
		settings['stage'] = settings['prod'].dup.merge!({
			:environment_name	=> 'stage',
			:service_path 		=> "c:/apps/materialstesting/#{convention_dir}_#{environment_name}",
			:staging_path		=> "c:/deployment/#{convention_dir}_#{environment_name}",			
			:service_dll		=>  "#{ src_app_dir }.dll",
			:service_name		=> "#{@app_name}_#{environment_name}",
			:db_name			=> "#{@app_name}_#{environment_name}",			
			:drop_db			=> true,
			:is_clean			=> true,
			:data_source		=> settings['prod'][:db_name],
			:clone_db			=> clone
		})
		
		return settings[environment_name]
	end
	
	def configure_tools(tools_dir,dist_dir)
		raise ArgumentError, 'directories must be configured first', caller if tools_dir.nil?
		tools = {
			:msdeploy 	=> 'C:/Program Files/IIS/Microsoft Web Deploy/msdeploy.exe',
			:sqlcmd 	=> 'C:/Program Files/Microsoft SQL Server/100/Tools/Binn/sqlcmd.exe',
			:teetool 	=> File.join(tools_dir,'cpptee','tee.exe'),
			:roundhouse => File.join(tools_dir,'roundhouse','rh.exe'),
			:powershell => File.join('C:/Windows/system32/WindowsPowerShell/v1.0/powershell.exe'),
			:xunit 		=> File.join(tools_dir,'xunit','xunit.console.clr4.exe'),
			:xunit35 	=> File.join(tools_dir,'xunit','xunit.console.exe'),
			:mongodb	=> File.join(tools_dir,'mongodb','mongo.exe'),
			:app		=> File.join(dist_dir,'tools','app','bin','Cei.Materials.Tools.exe') #the path to our app tools exe
		}
	end
	
end