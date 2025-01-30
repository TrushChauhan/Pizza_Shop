function addUser(){
    window.location.href = 'add-user.html';
}
function backToUsers(){
    window.location.href='users.html';
}
function editUser(){
    window.location.href='edit-user.html'
}
function deleteUserModal(){
    const ele=document.getElementById("modal-del");
    ele.style.display="flex";
    document.getElementById("entire-frame").style.opacity=0.7;
}
function removeModal(){
    const ele=document.getElementById("modal-del");
    ele.style.display="none";
    document.getElementById("entire-frame").style.opacity=1;
}