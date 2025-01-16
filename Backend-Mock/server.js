const express = require("express");
const multer = require("multer");
const cors = require("cors");
const path = require("path");
const fs = require("fs");
const { v4: uuidv4 } = require('uuid');

const app = express();
const PORT = 5000;

// Middleware CORS (pozwala na połączenie frontu z backendem)
app.use(cors());

// Ustawienie folderu, gdzie pliki będą przechowywane
const uploadFolder = path.join(__dirname, "uploads");
if (!fs.existsSync(uploadFolder)) {
  fs.mkdirSync(uploadFolder);
}

// Konfiguracja Multer do przesyłania plików
const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, uploadFolder); // Folder to store uploaded files
  },
  filename: (req, file, cb) => {
    // Preserve original file name, if you need uniqueness, add a timestamp
    const originalFileName = file.originalname;
    const filePath = path.join(uploadFolder, originalFileName);

    // Check if the file already exists
    if (fs.existsSync(filePath)) {
      // If file exists, add a timestamp to make it unique
      const uniqueSuffix = Date.now() + "-" + Math.round(Math.random() * 1e9);
      cb(null, uniqueSuffix + "-" + originalFileName);
    } else {
      // If file doesn't exist, use the original file name
      cb(null, originalFileName);
    }
  },
});
const upload = multer({ storage }).single("file"); // Ensure the field name is "file"

// Endpoint do uploadu plików
app.post("/api/files", upload, (req, res) => { // Use the configured upload middleware
  try {
    console.log("Received file:", req.file);

    const file = req.file;
    if (!file) {
      console.log("No file uploaded");
      return res.status(400).json({ message: "No file uploaded" });
    }

    const folderStructure = {
      id: uuidv4(),
      name: "uploads",
      subfolders: [],
      files: [],
    };

    const buildFolderStructure = (dirPath, folder) => {
      console.log(`Building folder structure for: ${dirPath}`);
      const items = fs.readdirSync(dirPath);
      items.forEach((item) => {
        const itemPath = path.join(dirPath, item);
        const stats = fs.statSync(itemPath);
        if (stats.isDirectory()) {
          console.log(`Found directory: ${item}`);
          const subfolder = {
            id: uuidv4(),
            name: item,
            subfolders: [],
            files: [],
          };
          folder.subfolders.push(subfolder);
          buildFolderStructure(itemPath, subfolder);
        } else {
          console.log(`Found file: ${item}`);
          folder.files.push({
            id: uuidv4(),
            name: item,
            url: `http://localhost:${PORT}/uploads/${item}`,
          });
        }
      });
    };

    buildFolderStructure(uploadFolder, folderStructure);

    res.status(200).json({
      message: "File uploaded successfully",
      fileName: file.filename,
      filePath: `/uploads/${file.filename}`,
      folderStructure,
    });
  } catch (error) {
    console.log("Error uploading file:", error);
    res.status(500).json({ message: "Error uploading file", error });
  }
});

// Endpoint do pobierania listy plików
app.get("/api/files", (req, res) => {
  try {
    const folderStructure = {
      id: uuidv4(),
      name: "uploads",
      subfolders: [],
      files: [],
    };

    const buildFolderStructure = (dirPath, folder) => {
      const items = fs.readdirSync(dirPath);
      items.forEach((item) => {
        const itemPath = path.join(dirPath, item);
        const stats = fs.statSync(itemPath);
        if (stats.isDirectory()) {
          const subfolder = {
            id: uuidv4(),
            name: item,
            subfolders: [],
            files: [],
          };
          folder.subfolders.push(subfolder);
          buildFolderStructure(itemPath, subfolder);
        } else {
          folder.files.push({
            id: uuidv4(),
            name: item,
            url: `http://localhost:${PORT}/uploads/${item}`,
          });
        }
      });
    };

    buildFolderStructure(uploadFolder, folderStructure);

    res.status(200).json(folderStructure);
  } catch (error) {
    res.status(500).json({ message: "Error reading files", error });
  }
});

// Serwowanie plików z folderu "uploads"
app.use("/uploads", express.static(uploadFolder));

// Start serwera
app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});