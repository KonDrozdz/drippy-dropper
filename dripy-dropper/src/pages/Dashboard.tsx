import React, { useState, useEffect, useCallback } from "react";
import { useQuery } from "react-query";
import axios from "axios";
import { Typography, Button, List, ListItem, ListItemText, TextField, Box } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useDropzone } from "react-dropzone";
import BackendNestedFileList from "../components/BackendNestedFileList"; // Import the new BackendNestedFileList component
import { Folder } from "../interfaces/Folder"; // Import the Folder interface from the new file
import { BackendApi } from "../interfaces/Api"; // Import the BackendApi enum

const Dashboard: React.FC = () => {
  const [filesAndFolders, setFilesAndFolders] = useState<Folder[]>([]); // Update the type of filesAndFolders
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]); // Zmienna stanu do przechowywania wybranych plików
  const [newFolderName, setNewFolderName] = useState<string>(""); // State for new folder name
  const [rootFolder, setRootFolder] = useState<Folder | null>(null); // State for root folder
  const navigate = useNavigate();

  useEffect(() => {
    const fetchDataOnLoad = async () => {
      await refetch();
    };

    fetchDataOnLoad();
  }, []);

  // Pobieranie plików i folderów
  const { data: fetchedData, refetch } = useQuery<any>("files", async () => {
    const token = localStorage.getItem("token");
    const ownerId = JSON.parse(atob(token!.split('.')[1])).sub; // Extract owner ID from token
    const response = await axios.get<any>(BackendApi.GET_FOLDERS_BY_USER, {
      headers: { Authorization: `Bearer ${token}` },
      params: {
        OwnerId: ownerId
      }
    });

    // Find the root folder (the one without a parentFolderId) and set its parentFolderId to null
    const rootFolder = response.data.find((folder: any) => !folder.parentFolderId);
    if (rootFolder) {
      rootFolder.parentFolderId = null;
    }

    // Fetch contents for each folder
    const fetchFolderContents = async (folderId: string) => {
      const folderResponse = await axios.get<any>(BackendApi.GET_FOLDER_CONTENTS, {
        headers: { Authorization: `Bearer ${token}` },
        params: {
          FolderId: folderId,
          OwnerId: ownerId
        }
      });
      return folderResponse.data;
    };

    const foldersWithContents = await Promise.all(
      response.data.map(async (folder: any) => {
        const contents = await fetchFolderContents(folder.folderId);
        return {
          ...folder,
          files: contents.files.map((file: any) => ({
            id: file.fileId,
            name: file.name,
            url: `${BackendApi.GET_FILE_DETAILS}/${file.fileId}`
          }))
        };
      })
    );
    // Filter out the root folder and folders with GUID as name
    const filteredFolders = foldersWithContents.filter((folder: any) => !/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(folder.name));
    setFilesAndFolders(filteredFolders); 
    console.log(filteredFolders);
    setRootFolder(rootFolder); // Set the root folder in state

    return foldersWithContents;
    });

    useEffect(() => {
    if (fetchedData) {
      // Filter out folders with GUID as name
      const filteredData = fetchedData.filter((folder: any) =>  !/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(folder.name));
      setFilesAndFolders(filteredData); // Ustawienie danych plików i folderów

    }
    }, [fetchedData]);

  // Obsługa wylogowania
  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/");
  };

  // Obsługa usuwania pliku
  const handleDeleteFile = async (fileId: string) => {
    const token = localStorage.getItem("token");
    const ownerId = JSON.parse(atob(token!.split('.')[1])).sub; // Extract owner ID from token
    await axios.delete(`${BackendApi.DELETE_FILE}/${fileId}`, {
      headers: { Authorization: `Bearer ${token}` },
      data: {
        fileId,
        ownerId: ownerId
      }
    });
    refetch(); // Refresh the file list after deletion
  };

  // Obsługa wyboru plików
  const onDrop = useCallback((acceptedFiles: File[]) => {
    setSelectedFiles(acceptedFiles);
  }, []);

  const { getRootProps, getInputProps } = useDropzone({ onDrop });

  // Obsługa przesyłania plików
  const handleUpload = async () => {
    if (selectedFiles.length === 0) return;

    const token = localStorage.getItem("token");
    const ownerId = JSON.parse(atob(token!.split('.')[1])).sub; // Extract owner ID from token

    try {
      for (const file of selectedFiles) {
        const formData:any = new FormData();
        formData.append("File", file);
        formData.append("FolderId", rootFolder?.folderId || ""); // Use root folder ID
        formData.append("OwnerId", ownerId);

        await axios.post(BackendApi.ADD_FILE, formData, {
          headers: {
            "Content-Type": "multipart/form-data",
            Authorization: `Bearer ${token}`,
          },
        });
      }
      refetch(); // Refresh the file list after upload
      setSelectedFiles([]); // Clear the selected files
    } catch (error) {
      console.error("Error uploading file:", error);
    }
  };

  // Obsługa tworzenia folderu
  const handleCreateFolder = async () => {
    if (!newFolderName) return;

    const token = localStorage.getItem("token");
    const ownerId = JSON.parse(atob(token!.split('.')[1])).sub; // Extract owner ID from token

    try {
      await axios.post(BackendApi.ADD_FOLDER, {
        name: newFolderName,
        parentFolderId: rootFolder?.folderId || null, // Use root folder ID
        ownerId: ownerId,
      }, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });
      refetch(); // Refresh the folder list after creation
      setNewFolderName(""); // Clear the folder name input
    } catch (error) {
      console.error("Error creating folder:", error);
    }
  };

  return (
    <>
      <Typography variant="h4" mb={2}>
        Twoje Pliki
      </Typography>
      <Button variant="outlined" color="error" onClick={handleLogout}>
        Wyloguj się
      </Button>

      <div {...getRootProps()} style={{ border: '2px dashed gray', padding: 16, textAlign: 'center', marginTop: 16 }}>
        <input {...getInputProps()} />
        <Typography>Przeciągnij i upuść pliki tutaj, lub kliknij aby wybrać pliki</Typography>
        <List>
          {selectedFiles.map((file) => (
            <ListItem key={file.name}>
              <ListItemText primary={file.name} />
            </ListItem>
          ))}
        </List>
      </div>
      <Button variant="contained" color="primary" onClick={handleUpload} disabled={selectedFiles.length === 0} style={{ marginTop: 16 }}>
        Prześlij pliki
      </Button>

      <Box mt={4}>
        <TextField
          label="Nazwa nowego folderu"
          value={newFolderName}
          onChange={(e) => setNewFolderName(e.target.value)}
          fullWidth
        />
        <Button variant="contained" color="primary" onClick={handleCreateFolder} style={{ marginTop: 16 }}>
          Utwórz folder
        </Button>
      </Box>

      {filesAndFolders.length > 0 && (
        //console.log(filesAndFolders),
        <BackendNestedFileList
          items={filesAndFolders} // Ensure the type matches the expected Folder type
          handleDeleteFile={handleDeleteFile}
        />
      )}
    </>
  );
};

export default Dashboard;