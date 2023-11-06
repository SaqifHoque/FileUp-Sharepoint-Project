function clearStorage() {
    localStorage.clear();
    sessionStorage.clear();
}

function downloadFile(fileName, content) {
    fetch(`/api/Files/downloadFile/${fileName}`)
        .then(response => response.blob())
        .then(blob => {
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', fileName);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        })
        .catch(error => {
            console.error('Download failed:', error);
        });
}