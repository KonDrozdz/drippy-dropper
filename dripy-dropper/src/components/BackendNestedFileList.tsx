import React, { useState } from 'react';
import { Box, Typography, List, ListItemText, ListItemAvatar, Avatar, IconButton, Collapse, ListItemButton, ListItemIcon, Drawer } from '@mui/material';
import FolderIcon from '@mui/icons-material/Folder';
import DeleteIcon from '@mui/icons-material/Delete';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import FileIcon from './FileIcon';
import { Folder, File } from "../interfaces/Folder";
import DocViewer, { DocViewerRenderers } from "@cyntler/react-doc-viewer";
import "@cyntler/react-doc-viewer/dist/index.css";

interface BackendNestedFileListProps {
  items: Folder[];
  handleDeleteFile: (fileId: string) => void;
}

const BackendNestedFileList: React.FC<BackendNestedFileListProps> = ({ items, handleDeleteFile }) => {
  const [openFolders, setOpenFolders] = useState<Set<string>>(new Set());
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);


  const handleFolderClick = (folderId: string) => {
    const newOpenFolders = new Set(openFolders);
    if (newOpenFolders.has(folderId)) {
      newOpenFolders.delete(folderId);
    } else {
      newOpenFolders.add(folderId);
    }
    setOpenFolders(newOpenFolders);
  };

  const fetchFileDetails = async (fileId: string) => {
    try {
      const response = await fetch(`/api/Files/Get-File-Details?fileId=${fileId}`);
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error fetching file details:', error);
      return null;
    }
  };

  const handleFileClick = async (file: File) => {
    const fileDetails = await fetchFileDetails(file.fileId);
    if (fileDetails) {
      setSelectedFile({ ...file, ...fileDetails });
      setDrawerOpen(true);
    }
  };

  const renderFolder = (folder: Folder) => {
    const isOpen = openFolders.has(folder.folderId);

    return (
      <div key={folder.folderId}>
        <ListItemButton onClick={() => handleFolderClick(folder.folderId)}>
          <ListItemIcon>
            <FolderIcon />
          </ListItemIcon>
          <ListItemText primary={folder.name} />
          {isOpen ? <ExpandLess /> : <ExpandMore />}
        </ListItemButton>
        <Collapse in={isOpen} timeout="auto" unmountOnExit>
          <List component="div" disablePadding>
            {folder.files?.map(renderFile)}
          </List>
        </Collapse>
      </div>
    );
  };

  const renderFile = (file: File) => (
    <ListItemButton key={file.fileId} sx={{ pl: 4 }} onClick={() => handleFileClick(file)}>
      <ListItemAvatar>
        <Avatar>
          <FileIcon fileName={file.name} />
        </Avatar>
      </ListItemAvatar>
      <ListItemText
        primary={file.name}
        secondary={<a href={file.path} target="_blank" rel="noopener noreferrer">Zobacz plik</a>}
      />
      <IconButton edge="end" onClick={() => handleDeleteFile(file.fileId)} sx={{ color: 'red' }}>
        <DeleteIcon />
      </IconButton>
    </ListItemButton>
  );

const renderFilePreview = (file: File) => {
    const fileExtension = file.name.split('.').pop()?.toLowerCase();
    let content;
    switch (fileExtension) {
        case 'jpg':
        case 'jpeg':
        case 'png':
        case 'gif':
            content = <img src={file.path} alt={file.name} style={{ width: '100%' }} />;
            break;
        case 'pdf':
        case 'csv':
            content = <DocViewer documents={[{ uri: file.path }]} pluginRenderers={DocViewerRenderers} />;
            break;
        case 'txt':
            content = <iframe src={file.path} width="100%" height="400px" title={file.name} />;
            break;
        default:
            content = <Typography>Nieobsługiwany format pliku</Typography>;
    }

    return (
        <Drawer
            anchor="right"
            open={drawerOpen}
            onClose={() => setDrawerOpen(false)}
            sx={{ '& .MuiDrawer-paper': { width: '40%' } }}
        >
            <Box sx={{ padding: 4 }}>
                <Typography variant="h6">{file.name}</Typography>
                {content}
            </Box>
        </Drawer>
    );
};

  return (
    <Box sx={{ padding: 4 }}>
      <Typography variant="h4" mb={2}>
        Pliki i Foldery
      </Typography>
      <List
        sx={{ width: '100%', maxWidth: 360, bgcolor: 'background.paper' }}
        component="nav"
        aria-labelledby="nested-list-subheader"
      >
        {items.map(renderFolder)}
      </List>
      {selectedFile && (
        <Box sx={{ marginTop: 4 }}>
          {renderFilePreview(selectedFile)}
        </Box>
      )}
    </Box>
  );
};

export default BackendNestedFileList;
