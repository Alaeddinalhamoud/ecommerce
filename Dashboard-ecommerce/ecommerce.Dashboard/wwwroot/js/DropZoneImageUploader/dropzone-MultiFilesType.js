Dropzone.options.dropzoneJsForm = {
    autoProcessQueue: true,
    maxFilesize: 1,
    maxFiles: 10,
    resizeWidth: 600,
    resizeHeight: 600,
    resizeMethod: 'crop', 
    acceptedFiles: '.jpg, .jpeg, .png, .pdf',
    init: function () {
        this.on("queuecomplete", function () {
            var submitButton = document.querySelector("#submit-all");            
            submitButton.classList.remove("d-none");
            submitButton.addEventListener("click", function () { 
                var btnProductSpecifications = document.getElementById("btn-ProductSpecifications");
                btnProductSpecifications.classList.remove("d-none");   
                var btnDone = document.getElementById("btn-done");
                btnDone.classList.remove("d-none"); 
                submitButton.classList.add("d-none");
            });           
        });
    }    
};