const API_URL = "/api/MetaData";

async function getDocuments(){
    const response = await fetch(API_URL);

    if(response.status === 204)
        return [];
    
    if(!response.ok)
        throw new Error("Failed to load documents.");

    return await response.json();
}

async function getDocumentById(id){
    if(!id)
        throw new Error("Document-ID is required.");

    const response = await fetch(`${API_URL}/${id}`);

    if(response.status === 204)
        return null;

    if(!response.ok)
        throw new Error("Document not found.");

    return await response.json();
}

async function createDocument(file, id, title, createdOn, modifiedLast, fileSize, fileType, summary){
    const formData = new FormData();
    formData.append("file", file);
    formData.append("id", id);
    formData.append("title", title);
    formData.append("summary", summary);
    formData.append("fileType", fileType);
    formData.append("fileSize", fileSize.toString());
    formData.append("createdOn", createdOn);
    formData.append("modifiedLast", modifiedLast);

    const response = await fetch(`${API_URL}/upload`, {
        method: "POST",
        body: formData
    });

    if(!response.ok)
        throw new Error("Failed to upload document.");

    return await response.json();
}


async function deleteDocument(id){
    if(!id)
        throw new Error("ID is required");

    const response = await fetch(`${API_URL}/${id}`, {
        method: "DELETE"
    });

    if(!response.ok)
        throw new Error("Failed to delete document.");

    return await response.json();
}

async function updateDocument(document){
    if(!document?.id)
        throw new Error("Document-ID is required.");

    const response = await fetch(`${API_URL}/${document.id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(document)
    });

    if(!response.ok)
        throw new Error("Failed to updated document.");

    return await response.json();
}

async function searchDocuments(query){
    if(!query || query.trim() === "")
        throw new Error("Suchbegriff darf nicht leer sein"); // das ist bullshit
    
    const url = new URL(`${API_URL}/search`, window.location.origin);
    url.searchParams.append("q", query);

    const response = await fetch(url);

    if(!response.ok)
        throw new Error("Failed to search document.");

    return await response.json();
}