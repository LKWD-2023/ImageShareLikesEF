$(() => {
    setInterval(() => {
        updateLikes();
    }, 1000);

    $("#like-button").on('click', function () {
        const id = $("#image-id").val();
        $.post('/home/likeimage', { id }, function () {
            updateLikes();
            $("#like-button").prop('disabled', true);
        });
    });

    function updateLikes() {
        const id = $("#image-id").val();
        $.get(`/home/getlikes`, { id }, function ({ likes }) {
            $("#likes-count").text(likes);
        });
    }
});
