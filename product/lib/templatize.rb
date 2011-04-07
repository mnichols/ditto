require 'erb'
require 'ostruct'

class Templatize  	
	
	def initialize params		
		@source_dir = params[:source_dir]
		@dest_dir = params[:dest_dir]
		@product_dir = params[:product_dir]
		@product_src_dir = params[:product_src_dir]	|| 'UNDEFINED_DLL_FROM_TEMPLATIZE'			
		
		puts "Templatize initialized with params @source_dir=#{@source_dir} ; @dest_dir = #{@dest_dir} ; @product_dir = #{@product_dir} ; @product_src_dir = #{ @product_src_dir }"
		
		@global_files = File.join(@source_dir,"global","**","*.{template}") unless params[:exclude_global]				
		@product_files = File.join(@source_dir,@product_dir,"**","*.{template}") unless params[:exclude_product]
		@expand_files = get_files(@global_files,@product_files) || {}
		
		super()
	end
	
	def execute	data		
		if (data.nil? || data.length==0 ) then
			puts 'WARN: data was not given for templatizing'
		end		
		@expand_files = @expand_files || {}		
		return if @expand_files.empty?		
		@data = data
		@expand_files.each {|template_file,output_file|
			expand_template template_file, output_file
		}		
	end

	private

	def rename_template(file,product_src_dir)
		output_name = File.basename(file).sub(".template","").sub("app.exe.config","#{product_src_dir}.exe.config").sub("app.config","#{product_src_dir}.dll.config")
		puts "renaming #{file} to #{output_name}"
		return output_name			
	end	
	
	def preserve_hierarchy(src,file)					
		target = file.pathmap("%{^#{src},#{@dest_dir}}p")		
		puts "target is #{target}, while its File.dirname is #{File.dirname(target)}"
		return File.dirname(target)
	end
	
	def get_files(global_files,prod_files)		
		globals = Hash.new
		prods = Hash.new
		globals = Hash[*FileList.new(global_files).collect { |file| [file, rename_template(file,@product_src_dir).in(@dest_dir)]}.flatten] unless global_files.nil?
		prods = Hash[*FileList.new(prod_files).collect { |file| [file, rename_template(file,@product_src_dir).in(preserve_hierarchy(File.join(@source_dir,@product_dir),file))]}.flatten] unless prod_files.nil?
		puts "found #{globals.length} global files to templatize"
		puts "found #{prods.length} product files to templatize"
		
		all_files = globals
		prods.each { |tmp,output| all_files[tmp]=output }
		return all_files
		#files = FileList.new()
		#files.include(global_files) unless global_files.nil?
		#files.include(prod_files) unless prod_files.nil?
		#
		#return Hash[*files.collect { |file| [file, rename_template(file,@product_src_dir).in(preserve_hierarchy(file))]}.flatten]		
	end
	
	def expand_template(template_file, output_file)
		puts "Parsing #{template_file} into #{output_file}"
		vars = OpenStruct.new @data	|| {}
		template = File.read(template_file)
		erb = ERB.new(template)				
		File.open(output_file,"w"){|output|
			output.write(erb.result(vars.send(:binding)))
		}		
	end	
end
