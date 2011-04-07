require File.join(File.dirname(__FILE__),'msbuild.rb')
require File.join(File.dirname(__FILE__),'templatize.rb')
require File.join(File.dirname(__FILE__),'roundhouse.rb')
require File.join(File.dirname(__FILE__),'zipdirectory.rb')

class Package
	
	def initialize		
		@pkg_dir = File.join(configatron.dir.dist,configatron.product.dir)
		@app_dir = File.join(@pkg_dir,'app')
		@bin_dir = File.join(@app_dir,'bin')
		@dbmigration_dir = File.join(@pkg_dir,'db')		
		@deploy_dir = File.join(@pkg_dir,'deploy')		
		@tools_dir = File.join(@pkg_dir,'tools')
	end
	
	def server					
		compile()
		tree()
		copy_win_app()
		configure(@bin_dir)
		db_migration()
		tools()
		package_app()
		package_dist()
	end

	def web				
		compile()
		tree()
		copy_web_app()
		configure(@app_dir)
		db_migration()
		package_app()
		package_dist()
	end
	
	def compile
		MSBuild.compile(
			:project => configatron.sln,
			:clrversion => configatron.clr,
			:target => 'Build',		
			:properties => {			
				:Configuration => configatron.build.config,
				:Disable_CopyWebApplication => "true",
				:AssemblyOriginatorKeyFile => configatron.build.key_file
			}
		)
	end
	
	def tree
		#setup directories
		mkdir_p @pkg_dir
		mkdir_p @app_dir
		mkdir_p @bin_dir
		mkdir_p @dbmigration_dir	
		mkdir_p @deploy_dir
		mkdir_p @tools_dir
	end
	
	def copy_win_app		
		dll_dir = File.join(configatron.dir.app,configatron.product.src_dir,'bin',configatron.build.config).to_absolute
		cp_r FileList.new(File.join(dll_dir,"*.{dll,pdb,xml,exe,licx}")), @bin_dir
	end
	
	def copy_web_app
		web_dir = File.join(configatron.dir.app,configatron.product.src_dir).to_absolute
		web_files = File.join(web_dir,"**","*.{spark,asax,aspx,ascx,ashx,png,jpg,js,css,bmp,ico,gif,xml,licx}")
		dll_files = File.join(web_dir,"bin","*.{dll,pdb,xml}")		
		all_files = FileList.new(web_files,dll_files).exclude(File.join(web_dir,"obj")).exclude(File.join(web_dir,"logs"))
		all_files.copy_hierarchy(:source_dir => web_dir,:target_dir => @app_dir)			
	end
	
	def configure cfg_dir
		templatize = Templatize.new(
			:source_dir  		=> configatron.dir.config,
			:dest_dir 	 		=> cfg_dir,
			:product_dir 		=> configatron.product.dir,		
			:product_src_dir	=> configatron.product.src_dir
		)	
		
		templatize.execute configatron.env.to_hash
	end
	
	def db_migration
		#sql files, if any
		src_dir =Dir.glob(File.join(configatron.dir.db,configatron.product.dir))				
		if(FileList.new(File.join(configatron.dir.db,configatron.product.dir,"**","*.{sql,js}")).length == 0 ) then 					
			puts "No sql files exist for migration of product #{ configatron.product.dir }"
			return
		end
		#copy roundhouse
		cp_r File.dirname(configatron.tools.roundhouse),File.join(@dbmigration_dir,"roundhouse")		
		
		
		#copy mongodb
		cp_r File.dirname(configatron.tools.mongodb),File.join(@dbmigration_dir,"mongodb")		
		
		#copy migration files => note we use a regex for exclude so that the file doesn't have to exist to be excluded
		FileList.new(File.join(src_dir,"**","*.*")).exclude(/.template$/).copy_hierarchy(:source_dir=>src_dir,:target_dir => @dbmigration_dir)
	
		#templatize db version => db
		templatize = Templatize.new(
			:source_dir  		=> configatron.dir.db,
			:dest_dir 	 		=> @dbmigration_dir,
			:product_dir 		=> configatron.product.dir,
			:extension			=> 'template' #lest we choke
		)
		
		templatize.execute configatron.product.to_hash.merge!(configatron.deploy.to_hash)		
		
	end
	
	def tools
		#copy Application Tools app
		cp_r  File.dirname(configatron.tools.app),@tools_dir
	end
	
	def package_app
		zipper = ZipDirectory.new(
			:directories_to_zip => @pkg_dir,
			:output_file => "#{ configatron.product.dir }.zip",
			:output_path => @pkg_dir
		)
		zipper.package()		
	end
	
	def package_dist
		template_data = configatron.deploy.to_hash.merge!(configatron.tools.to_hash)	
		
		#copy ps1 and dependent deploy scripts into deploy director
		deploy_scripts = Templatize.new(
			:source_dir  		=> configatron.dir.deploy,
			:dest_dir 	 		=> @deploy_dir,
			:product_dir 		=> configatron.product.dir, 
			:product_src_dir	=> configatron.product.src_dir,
			:exclude_product	=> false, # changed to include product folders...MSDeploy.cmd will be invalid when run this way tho
			:exclude_global		=> false,
			:extension			=> 'template'
		)			
		deploy_scripts.execute template_data		
		
		#copy msdeploy.cmd into base deploy directory
		msdeploy_cmd = Templatize.new(
			:source_dir  		=> configatron.dir.deploy,
			:dest_dir 	 		=> @pkg_dir,
			:product_dir 		=> configatron.product.dir,		
			:product_src_dir	=> configatron.product.src_dir,
			:extension			=> 'template',
			:exclude_global		=> true,
			:exclude_product	=> false
		)
		msdeploy_cmd.execute template_data
		
		copy_non_templates()
		
		deployment = ZipDirectory.new(
			:directories_to_zip => @pkg_dir,
			:output_file => "#{ configatron.product.dir }_deploy.zip",
			:output_path => @pkg_dir
		)
		deployment.package()
		
		distribution = ZipDirectory.new(
			:directories_to_zip => @pkg_dir,
			:output_file => "#{configatron.product.dir}-#{configatron.product.package_number}.zip",
			:output_path => configatron.dir.dist
		)
		distribution.package()
	end
	
	def copy_non_templates
		FileList.new(File.join(configatron.dir.deploy, 'global','**','*'),File.join(configatron.dir.deploy, configatron.product.dir,'**','*')).reject {|file| ['.template'].include?(File.extname(file)) }.each do |file|
			cp file,@deploy_dir
		end		
	end	
end