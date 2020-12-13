#!/usr/bin/env bash


####################################
### Set up your build parameters ###
####################################

# the relative output directory of `dotnet publish`
BUILD_DIR=publish

# the target OS for `dotnet publish`
BUILD_TARGET=linux-x64

# the name of the archive with its extension
FILENAME=timestamp.tar

# the SSH credential name for the remote server
# I have this stored as $CL_BLOG_CREDENTIAL on my dev machine
# so you'll have to change this for your use
CREDENTIAL=$CL_BLOG_CREDENTIAL


###########################
### Clean up old builds ###
###########################

printf "Removing previous build...\n"
rm -r $BUILD_DIR
rm $FILENAME
printf "...done\n"


#############################
### Build the application ###
#############################

printf "Creating executable...\n"
dotnet publish -c release -o $BUILD_DIR -r $BUILD_TARGET --self-contained false
printf "...done\n"


###############################
### Archive the application ###
###############################

printf "Packaging for deployment...\n"
tar -rf $FILENAME $BUILD_DIR
printf "...done\n"


##################################################
### Remove the old application from the server ###
##################################################

printf "Clearing old deployment from server...\n"
ssh $CREDENTIAL "rm -r $BUILD_DIR"
printf "...done\n"


################################################
### Deploy the new application to the server ###
################################################

printf "Uploading to server...\n"
scp $FILENAME $CL_BLOG_CREDENTIAL:
printf "...done\n"


###########################
### Extract the archive ###
###########################

printf "Extracting on server...\n"
ssh $CREDENTIAL "tar -xf $FILENAME"
printf "...done\n"


##########################################
### Remove the archive from the server ###
##########################################

printf "Removing deployment archive from server...\n"
ssh $CREDENTIAL "rm $FILENAME"
printf "...done\n"


############
### Done ###
############

echo "To run Timestamp, log into remote server and execute ~/$BUILD_DIR/timestamp"