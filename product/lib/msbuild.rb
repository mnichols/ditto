class MSBuild
	def self.compile params
		version = params[:clrversion] || 'v3.5'
		
		projectFile = params[:project].to_absolute
		userProperties = params[:properties] || {}
		userSwitches = params[:switches] || {}		
		frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', version)
		msbuild = File.join(frameworkDir, 'msbuild.exe')
		
		properties = {
			:BuildInParallel => false,
			:BuildRunner => 'Rake',
			:Configuration => 'Debug'
		}
		properties.merge!(userProperties)
		
		properties = properties.collect { |key, value|
			"/property:#{key}=#{value}"
		}.join " "
		
		
		switches = {
			:nologo => true,
			:maxcpucount => true,
			:verbosity => 'minimal',
			:target => params[:target] || userSwitches[:target] || 'Build'
		}
		switches.merge!(userSwitches)
		
		switches = switches.collect { |key, value|
			"/#{key}#{":#{value}" unless value.kind_of? TrueClass or value.kind_of? FalseClass}" if value
		}.join " "
		
		sh "#{msbuild.escape} #{projectFile.escape} #{switches} #{properties}"
	end
end