import os
from sys import stdout, argv
import shutil
from glob import glob
import ntpath

# Use an environmental variable
projectDirs = [ 'C:/Development/Unity', 'C:/Development/Unity Projects' ]

dllPath = argv[1]

fileName = os.path.basename(dllPath)

filePattern = '*/Assets/**/%s' % fileName
fileDirPattern = '*/Assets/*/**/%s' % fileName


projectRootFolder = 'D:/Programs/Unity3d/Workspace/'

destinationProjects = [ 'Game Portal' ]

deployedDlls = []

for projectDir in projectDirs:

    path = os.path.join(projectDir, fileDirPattern)
    for file in glob(path):
        deployedDlls.append(file)
    
    path = os.path.join(projectDir, filePattern)
    for file in glob(path):
        deployedDlls.append(file)


for path in deployedDlls:
    print('Updating : %s' % path)

    if os.path.exists( path ):
        os.remove(path)
    
    shutil.copyfile(dllPath, path)

