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
const ensureIds = (items) => {
    items.forEach(item => {
      if (!item.id) {
        item.id = Math.random().toString(36).substring(2, 9);
      }
    });
  };
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
      return res.status(400).json({ message: "No file uploaded" });
    }
    res.status(200).json({
      message: "File uploaded successfully",
      fileName: file.filename,
      filePath: `/uploads/${file.filename}`,
    });
  } catch (error) {
    res.status(500).json({ message: "Error uploading file", error });
  }
});

// Endpoint do pobierania listy plików
app.get("/api/files", (req, res) => {
  fs.readdir(uploadFolder, (err, files) => {
    if (err) {
      return res.status(500).json({ message: "Error reading files", error: err });
    }
    const fileList = files.map((file) => ({
      id: uuidv4(),
      name: file,
      url: `http://localhost:${PORT}/uploads/${file}`,
    }));
    res.status(200).json(fileList);
  });
});

// Serwowanie plików z folderu "uploads"
app.use("/uploads", express.static(uploadFolder));

// Start serwera
app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});
