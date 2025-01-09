import React, { useState, useEffect, useCallback } from "react";
import { useQuery } from "react-query";
import axios from "axios";
import { Typography, Button, List, ListItem, ListItemText } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useDropzone } from "react-dropzone";
import NestedFileList from "../components/NestedFileList"; // Import the new NestedFileList component

const Dashboard: React.FC = () => {
  const [filesAndFolders, setFilesAndFolders] = useState<any[]>([]); // Zmienna stanu do przechowywania plików i folderów
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]); // Zmienna stanu do przechowywania wybranych plików
  const navigate = useNavigate();

  // Pobieranie plików i folderów
  const { data: fetchedData, refetch } = useQuery<any>("files", async () => {
    const token = localStorage.getItem("token");
    const response = await axios.get<any>("http://localhost:5000/api/files", {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
  });

  useEffect(() => {
    if (fetchedData) {
      setFilesAndFolders(fetchedData); // Ustawienie danych plików i folderów
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
    await axios.delete(`http://localhost:5000/api/files/${fileId}`, {
      headers: { Authorization: `Bearer ${token}` },
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

    try {
      for (const file of selectedFiles) {
        const formData:any = new FormData();
        formData.append("file", file);

        await axios.post("http://localhost:5000/api/files", formData, {
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

      <NestedFileList
        items={filesAndFolders}
        handleDeleteFile={handleDeleteFile}
      />
    </>
  );
};

export default Dashboard;