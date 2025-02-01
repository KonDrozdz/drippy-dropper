export enum BackendApi {
    ADD_FILE = 'http://localhost:5052/api/Files/Add-File',
    UPDATE_FILE = 'http://localhost:5052/api/Files/Update-File',
    DELETE_FILE = 'http://localhost:5052/api/Files/Delete-File',
    GET_FILE_DETAILS = 'http://localhost:5052/api/Files/Get-File-Details',
    GET_FILES_BY_USER = 'http://localhost:5052/api/Files/Get-Files-By-User',
    DOWNLOAD_FILE = 'http://localhost:5052/api/Files/Download-File',
    ADD_FOLDER = 'http://localhost:5052/api/Folders/Add-Folder',
    UPDATE_FOLDER = 'http://localhost:5052/api/Folders/Update-Folder',
    DELETE_FOLDER = 'http://localhost:5052/api/Folders/Delete-Folder',
    GET_FOLDER_CONTENTS = 'http://localhost:5052/api/Folders/Get-Folder-Contents',
    GET_FOLDERS_BY_USER = 'http://localhost:5052/api/Folders/Get-Folders-By-User',
    LOGIN = 'http://localhost:5052/api/Users/Login',
    REGISTER = 'http://localhost:5052/api/Users/Register',
    GET_USER_DETAILS = 'http://localhost:5052/api/Users/Get-User-Details'
}