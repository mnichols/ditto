class XUnit

  def initialize params
    @exe = params[:exe]
	@assemblies = params[:assemblies] #array
	@assembly = params[:assembly]	
    @options = params[:options]
	@html_output = params[:html_output]
	@working_directory = params[:working_directory] || Dir.pwd	
    super()
  end

  def get_command_line
    command_params = []
    command_params << @exe
    command_params << get_command_parameters
    commandline = command_params.join(" ")    
    commandline
  end
  
  def get_command_parameters
    command_params = []	
    command_params << @options.join(" ") unless @options.nil?
    command_params << build_html_output unless @html_output.nil?
    command_params
  end

  def execute()    		
	puts 'executing xunit'
    @assemblies = [] if @assemblies.nil?
    @assemblies << @assembly unless @assembly.nil?
	raise 'At least one assembly is required for assemblies attr' if @assemblies.length==0	    
	
    set_working_directory
    @assemblies.each do |assm|
      command_params = get_command_parameters.collect{ |p| p % File.basename(assm) }
      command_params.insert(0,assm)	
	  cmd = "#{@exe.escape} #{command_params.join(' ')}"
	  puts "running xunit with : #{cmd}"
	  result = sh cmd
      raise 'xunit failed' if !result
    end       	
	reset_working_directory	
  end

  def build_html_output			
    fail_with_message 'Directory is required for html_output' if !File.directory?(File.expand_path(@html_output))
    "/html #{File.join(File.expand_path(@html_output),"%s.html")}" 	
  end
  
  private 
	def set_working_directory
		@original_directory = Dir.pwd
		return if @working_directory == @original_directory
		Dir.chdir(@working_directory)
	end
  
	def reset_working_directory
		return if Dir.pwd == @original_directory
		Dir.chdir(@original_directory)
	end
end
