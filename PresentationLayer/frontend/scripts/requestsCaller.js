(async () => { const result = await fetchDocuments(); })();

async function fetchDocuments(){
    const result = await getDocuments();

    if(result.length < 1)
        return;

    for(let i = 0; i < result.length; i++)
        addNote(result[i].id, result[i].title, result[i].createdOn, result[i].modifiedLast, result[i].fileSize, result[i].fileType, result[i].summary);

    return result;
}

async function uploadFile(){
    if(!fileInput.files[0]){
        alert("Please select a document.");
        return;
    }

    for(let i = 0; i < fileInput.files.length; i++){
        let id = crypto.randomUUID();
        try{
            var lastModified = new Date(fileInput.files[i].lastModified).toLocaleString("en-EN")
            await createDocument(document.getElementById("file-upload").files[i], id, fileInput.files[i].name, lastModified, lastModified, fileInput.files[i].size, fileInput.files[i].type, "No Summary");
            addNote(id, fileInput.files[i].name, lastModified, lastModified, fileInput.files[i].size, fileInput.files[i].type, "Waiting for OCR to finish ...");
        }catch(error){
            console.error("Upload failed:", error);
            alert(`Upload failed for: "${fileInput.files[i].name}"`);
        }
    } 
}

async function deleteNote(documentID){
    var check = prompt("Please insert the ID of the document you want to delete:");
    if(check === null)
        return

    if(check != documentID){
        alert("IDs do not match.");
        return;
    }

    const result = await deleteDocument(documentID);
    if(result.deletedId === documentID)
        document.getElementById(documentID).remove();

    let documentsDiv = document.getElementById("documents");
    if(documentsDiv.children.length === 1 && documentsDiv.querySelector("#no-documents"))
        documentsDiv.querySelector("#no-documents").classList.remove("hide");
}

async function updateNote(){
    var documentID = document.getElementById("edit-doc").value;
    var doc = document.getElementById(documentID);

    var newFilename = document.getElementById("edit-filename").value; 
    var oldFilename = doc.querySelector(".document-filename").innerText;
    var filesize = doc.querySelector(".document-size").innerText;
    var filetype = doc.querySelector(".document-type").innerText;
    var summary = document.getElementById("edit-summary").value;

    try{
        const result = await updateDocument( {id: documentID, title: newFilename, fileType: filetype, fileSize: filesize, summary: summary, objectName: `${documentID}_${newFilename}`} );
        updateDetails(documentID, newFilename, summary);
        closeModal();
    }catch(error){
        alert(`Failed to update document: ${filename}`);
        console.log(error);
    }
}

const searchBar = document.getElementById("searchbar");
searchBar.addEventListener("input", async () => {
    const result = await searchDocuments(searchBar.value);
    
    var documents = document.querySelectorAll("#documents .document");

    if(result === null){
        documents.forEach(doc => doc.classList.remove("hide"));
        return;
    }
    
    var visibleIds = new Set(result.map(doc => doc.id));
    documents.forEach(doc => { doc.classList.toggle("hide", !visibleIds.has(doc.id)); });
});