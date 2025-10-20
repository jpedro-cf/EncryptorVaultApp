export function dragDrop(onChange){
    const dragDrop = document.getElementById("drag-drop");
    const fileInput = dragDrop.querySelector("#drag-drop-file-input")
    
    dragDrop.addEventListener("dragover", (e) => {
        e.preventDefault()
        dragDrop.classList.add('active');
    });
    
    dragDrop.addEventListener("dragleave", (e) => {
        e.preventDefault()
        dragDrop.classList.remove('active');
    });
    
    
    dragDrop.addEventListener("drop", (e) => {
        e.preventDefault();
        dragDrop.classList.remove("active");
        
        const files = e.dataTransfer.files;
        if(!files){
            return
        }

        onChange(files)
    });
    
    dragDrop.addEventListener("click", (e) => {
        fileInput.click()
    })
    
    dragDrop.addEventListener("keydown", (e) => {
        if (e.key === 'Enter') {
            fileInput.click()
        }
    })
    
    fileInput.addEventListener("change", (e) => {
        if(!e.target.files){
            return
        }
        onChange(e.target.files)
    })
}
