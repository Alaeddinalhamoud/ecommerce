Dropzone.options.dropzoneJsForm = {
    autoProcessQueue: true,
    maxFilesize: 1,
    maxFiles: 1,
    acceptedFiles: '.jpg, .jpeg, .png',
    resizeMethod: 'crop',
    init: function () {
        this.on("queuecomplete", function () {
            var submitButton = document.querySelector("#submit-all");            
            submitButton.classList.remove("d-none");
            submitButton.addEventListener("click", function () {
                var btnDone = document.getElementById("btn-done");
                btnDone.classList.remove("d-none");
                submitButton.classList.add("d-none");
            });           
        }); 
    }    
};