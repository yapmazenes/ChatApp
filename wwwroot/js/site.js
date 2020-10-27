debugger;;
var createRoomBtn = document.getElementById('create-room-btn');
var createRoomModal = document.getElementById('create-room-modal');

if (createRoomBtn != null) {

createRoomBtn.addEventListener('click', function () {
    createRoomModal.classList.add('active')
})
}

var closeModal = function () {
    createRoomModal.classList.remove('active')
}