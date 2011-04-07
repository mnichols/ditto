require File.join(File.dirname(__FILE__),'msbuild.rb')
require File.join(File.dirname(__FILE__),'templatize.rb')
require File.join(File.dirname(__FILE__),'roundhouse.rb')
require File.join(File.dirname(__FILE__),'xunit.rb')

class Testrunner
	def initialize
	end
	
	def run 
		if( !configatron.build.tests ) then
			return #no runny
		end
			
		mkdir configatron.dir.build unless File.exists? configatron.dir.build
		compile()
		test_assemblies = FileList.new(File.join(configatron.dir.build,"**","*{Specs.dll,IntegrationTests.dll}"))
		if test_assemblies.length == 0 then
			puts 'Test assemblies not found for testing'
			return
		end
		configure()
		db()
		
		#testrun
		test_opts = ["/noshadow"]
		test_opts << ["/teamcity"] if configatron.build.team_city
		
		
		xunit = XUnit.new(
			:exe 				=> configatron.tools.xunit,
			:assemblies 		=> test_assemblies,
			:options 			=> test_opts,
			:html_output 		=> configatron.dir.build,
			:command_directory	=> configatron.dir.build	
		)
		xunit.execute()
	end
	
	private 
	def compile
		MSBuild.compile(
			:project => configatron.sln,
			:clrversion => configatron.clr,
			:target => 'Build',			
			:properties => {
				:Configuration => configatron.build.config,
				:OutDir => File.join(configatron.dir.build,"/").to_absolute + "/" ,
				:Disable_CopyWebApplication => "true",
				:AssemblyOriginatorKeyFile => configatron.build.key_file
				# :PlatformTarget => configatron.build.team_city ? 'x86' : 'AnyCPU' # removing this property for now...let's see what happens
			}
		)
	end
	
	def configure
		#expand config files
		templatize = Templatize.new(
			:source_dir  		=> configatron.dir.config,
			:dest_dir 	 		=> configatron.dir.build,
			:product_dir 		=> configatron.product.dir,		
			:product_src_dir	=> configatron.product.src_dir
		)		
		templatize.execute configatron.test.to_hash
	end
	
	def db
		#setup db
		roundhouse = Roundhouse.new(
			:db_dir  		=> configatron.dir.db,
			:dest_dir 	 	=> configatron.dir.build,
			:product_dir 	=> configatron.product.dir,
			:env 			=> configatron.test.environment,
			:database		=> "#{configatron.test.service_name}_#{configatron.test.environment}",
			:roundhouse		=> configatron.tools.roundhouse
		)
		roundhouse.drop configatron.product.to_hash
		roundhouse.create configatron.product.to_hash
	end
		
end