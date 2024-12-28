$(document).ready(function() {
    $('#uploadButton').click(function() {
        var file = $('#fileUpload')[0].files[0];
        if (!file) {
            alert('Please select a file first.');
            return;
        }

        var chunkSize = 1 * 1024 * 1024; // 1 MB
        var totalChunks = Math.ceil(file.size / chunkSize);

        function uploadChunk(chunkIndex) {
            var start = chunkIndex * chunkSize;
            var end = Math.min(start + chunkSize, file.size);
            var chunk = file.slice(start, end);

            var formData = new FormData();
            formData.append('file', chunk);
            formData.append('chunkIndex', chunkIndex);
            formData.append('totalChunks', totalChunks);

            $.ajax({
                url: service + 'file/upload',
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function(response) {
                    console.log(`Chunk ${chunkIndex + 1} uploaded successfully`);
                    if (chunkIndex + 1 < totalChunks) {
                        uploadChunk(chunkIndex + 1);
                    } else {
                        alert('File uploaded successfully');
                    }
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.error(`Chunk ${chunkIndex + 1} upload failed`);
                    alert('File upload failed');
                }
            });
        }

        uploadChunk(0);
    });
});