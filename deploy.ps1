####################################
### Set up your build parameters ###
####################################

# the relative output directory of `dotnet publish`
$build_dir = "publish"

# the target OS for `dotnet publish`
$build_target = "linux-x64"

# the name of the archive with its extension
$filename = "timestamp.zip"

# the SSH credential name for the remote server
# I have this stored as $env:CL_BLOG_CREDENTIAL on my dev machine
# so you'll have to change this for your use
$credential = $env:CL_BLOG_CREDENTIAL

# the SCP deployment credential:path for the remote server
# PowerShell doesn't allow string interpolation the way BASH does
# so the simplest solution is to have this as its own variable
$remote_deploy_path = "{0}:" -f $credential


###########################
### Clean up old builds ###
###########################

Write-Host "Removing previous build..."
Remove-Item $build_dir -Recurse
Remove-Item $filename
Write-Host "...done"


#############################
### Build the application ###
#############################

Write-Host "Creating executable..."
dotnet publish -c release -o $build_dir -r $build_target --self-contained false
Write-Host "...done"


###############################
### Archive the application ###
###############################

Write-Host "Packaging for deployment..."

# Note: Compress-Archive is bugged on Linux systems
# Files are archived with 000 permissions, and
# running `sudo chmod -R 644 publish` will
# just corrupt the files after extraction
Compress-Archive -Path $build_dir -DestinationPath $filename
Write-Host "...done"


##################################################
### Remove the old application from the server ###
##################################################

Write-Host "Clearing old deployment from server..."
ssh $credential "rm -r publish"
Write-Host "...done"


################################################
### Deploy the new application to the server ###
################################################

Write-Host "Uploading to server..."
scp $filename $remote_deploy_path
Write-Host "...done"


###########################
### Extract the archive ###
###########################

Write-Host "Extracting on server..."
ssh $credential "unzip $filename"
Write-Host "...done"


##########################################
### Remove the archive from the server ###
##########################################

Write-Host "Removing deployment archive from server..."
ssh $credential "rm $filename"
Write-Host "...done"


############
### Done ###
############

Write-Host "To run Timestamp, log into remote server and execute ~/$build_dir/timestamp"