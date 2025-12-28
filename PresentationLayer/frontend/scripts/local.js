(async () => { const result = await fetchDocuments(); })();

async function fetchDocuments(){
    const result = await getDocuments();

    if(result.length < 1)
        return;

    for(let i = 0; i < result.length; i++)
        addNote(result[i].id, result[i].title, result[i].createdOn, result[i].modifiedLast, result[i].fileSize, result[i].fileType, result[i].summary);

    return result;
}

function showDetails(documentID){
    var doc = document.getElementById(documentID);
    doc.querySelector(".document-details").classList.remove("hide");
    doc.querySelector(".buttons-grouping").classList.remove("hide");
    doc.querySelector(".hide-details").classList.remove("hide");
    doc.querySelector(".show-details").classList.add("hide");
}

function hideDetails(documentID){
    var doc = document.getElementById(documentID);
    doc.querySelector(".document-details").classList.add("hide");
    doc.querySelector(".buttons-grouping").classList.add("hide");
    doc.querySelector(".hide-details").classList.add("hide");
    doc.querySelector(".show-details").classList.remove("hide");
}

async function uploadFile(){
    if(!fileInput.files[0]){
        alert("Please select a document.");
        return;
    }

    for(let i = 0; i < fileInput.files.length; i++){
        let id = crypto.randomUUID();
        try{
            await createDocument(document.getElementById("file-upload").files[i], id, fileInput.files[i].name, new Date(fileInput.files[i].lastModified).toISOString(), new Date(fileInput.files[i].lastModified).toISOString(), fileInput.files[i].size, fileInput.files[i].type, "No Summary");
            addNote(id, fileInput.files[i].name, new Date(fileInput.files[i].lastModified).toISOString(), new Date(fileInput.files[i].lastModified).toISOString(), fileInput.files[i].size, fileInput.files[i].type, "Waiting for OCR to finish ...");
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
        document.getElementById(documentID).remove();console.log(document.getElementById("documents"));

    let documentsDiv = document.getElementById("documents");
    if(documentsDiv.children.length === 1 && documentsDiv.querySelector("#no-documents"))
        documentsDiv.querySelector("#no-documents").classList.remove("hide");

}

function createGrouping(detail, id){
    var groupingDiv = document.createElement("div");
    groupingDiv.className = "grouping";

    var label = document.createElement("label");
    label.innerText = detail + ":\u00A0";

    var div = document.createElement("div");
    div.innerText = id;

    groupingDiv.append(label, div);
    return groupingDiv;
}

function addNote(documentID, fileName, fileCreationDate, fileModificationDate, fileSize, fileType, fileSummary){
    var documentsWrapper = document.getElementById("documents");
    document.getElementById("no-documents").classList.add("hide");

    var doc = document.createElement("div");
    doc.className = "document";
    doc.id = documentID;

    var documentTitleDiv = document.createElement("div");
    documentTitleDiv.className = "document-title";

    var documentTitleH3 = document.createElement("h3");
    documentTitleH3.className = "document-filename";
    documentTitleH3.innerText = fileName;

    var buttonsGroupingDiv = document.createElement("div");
    buttonsGroupingDiv.className = "buttons-grouping hide";

    var deleteButton = document.createElement("button");
    deleteButton.className = "delete-button";
    deleteButton.textContent = "Delete";
    deleteButton.onclick = () => deleteNote(documentID);

    var updateButton = document.createElement("button");
    updateButton.className = "update-button";
    updateButton.textContent = "Update";
    updateButton.onclick = () => deleteNote(documentID);

    buttonsGroupingDiv.append(deleteButton, updateButton);

    documentTitleDiv.append(documentTitleH3, buttonsGroupingDiv);
    doc.append(documentTitleDiv);

    var showDetailsSpan = document.createElement("span");
    showDetailsSpan.className = "show-details";
    showDetailsSpan.innerText = "Details ↓";
    showDetailsSpan.onclick = () => showDetails(documentID);

    doc.append(showDetailsSpan);

    var documentDetailsDiv = document.createElement("div");
    documentDetailsDiv.className = "document-details hide";

    var rowDiv1 = document.createElement("div");
    rowDiv1.className = "row";
    
    var rowDiv1GroupingDiv1 = createGrouping("ID", documentID);
    
    rowDiv1.append(rowDiv1GroupingDiv1);
    documentDetailsDiv.append(rowDiv1);

    var rowDiv2 = document.createElement("div");
    rowDiv2.className = "row";

    var rowDiv1GroupingDiv1 = createGrouping("Created", fileCreationDate);
    var rowDiv1GroupingDiv2 = createGrouping("Updated", fileModificationDate);
    var rowDiv1GroupingDiv3 = createGrouping("Filesize", fileSize);
    var rowDiv1GroupingDiv4 = createGrouping("Filetype", fileType);

    rowDiv2.append(rowDiv1GroupingDiv1, rowDiv1GroupingDiv2, rowDiv1GroupingDiv3, rowDiv1GroupingDiv4);
    documentDetailsDiv.append(rowDiv2);

    var documentSummaryDiv = document.createElement("div");
    documentSummaryDiv.className = "document-summary";

    var summaryLabel = document.createElement("label");
    summaryLabel.innerText = "Summary: \n"

    var summarySpan = document.createElement("span");
    summarySpan.innerText = fileSummary;

    documentSummaryDiv.append(summaryLabel, summarySpan);
    documentDetailsDiv.append(documentSummaryDiv);
    doc.append(documentDetailsDiv);

    var hideDetailsSpan = document.createElement("span");
    hideDetailsSpan.className = "hide-details hide";
    hideDetailsSpan.innerText = "Details ↑";
    hideDetailsSpan.onclick = () => hideDetails(documentID);

    doc.append(hideDetailsSpan);
    documentsWrapper.append(doc);
}

const dropzone = document.getElementById("dropzone");
const fileInput = document.getElementById("file-upload");
const fileName = document.getElementById("file-name");
const searchBar = document.getElementById("searchbar");

dropzone.addEventListener("dragover", (e) => { // (changes style when files are dragged over the file dropzone)
    e.preventDefault();                        // prevents default behaviour of the browser for this event
    dropzone.classList.add("dragover");        // adds class for the dropzone for css styling
});

dropzone.addEventListener("dragleave", () => { // (changes style back to previous when files are not over the dropzone anymore)
    dropzone.classList.remove("dragover");     // removes class for the dropzone for css styling
});

function changeFileNameSpanOnUpload(files){
    if(fileName.innerText === "No file selected")
        fileName.innerText = "";

    for(let i = 0; i < files.length; i++){
        if(fileName.innerText.includes(files[i].name)){
            continue;
        }else
            fileName.innerText += files[i].name + "\n";
    }
}

dropzone.addEventListener("drop", (e) => { // (what happens when files are selected by being dropped)
    e.preventDefault();                    // prevents default behaviour of the browser for this event
    dropzone.classList.remove("dragover"); // removes class for the dropzone for css styling

    const files = e.dataTransfer.files;
    fileInput.files = files;

    changeFileNameSpanOnUpload(files);
});

fileInput.addEventListener("change", () => {     // (what happens when files are selected by being chosen by pressing the button)
    changeFileNameSpanOnUpload(fileInput.files);
});

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