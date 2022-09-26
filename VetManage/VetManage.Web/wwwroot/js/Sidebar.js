$(document).ready(function () {
    let sidebar = $(".sidebar");
    let sidebarItems = $(".sidebar-item")
    let burgerContainer = $(".menu-burger-container")
    let content = $(".content");

    checkSidebarCollapsed(sidebar);

    $("#burgerMenu").click(function () {
        if (sidebar.hasClass("open")) {
            collapseSideBar(sidebar, content);
        } else {
            openSideBar(sidebar, content);
        }
        console.log(sidebarItems);
    });

    sidebarItems.click(function () {
        $(this).addClass("active");
    });

    //$("#homeNav").click(function () {
    //    content.load("/Home/IndexPartial");
    //});

    //$("#ownersNav").click(function () {
    //    content.load("/Owners/IndexPartial");
    //});

    //$("#usersNav").click(function () {
    //    content.load("/Users/IndexPartial");
    //});

    //$("#vetsNav").click(function () {
    //    content.load("/Vets/IndexPartial");
    //});
    
    //$("#petsNav").click(function () {
    //    content.load("/Pets/Index");
    //});

    //$("#petsEdit").click(function () {
    //    console.log("yoyoyoyo");
    //    content.load("/Pets/Edit");
    //});
});

function openSideBar(sidebar, content) {
    localStorage.setItem("menuState", "open");

    sidebar.addClass("open");
    sidebar.removeClass("closed");

    content.addClass("open");
    content.removeClass("closed");

    $(".sidebar-item").addClass("flex-row");
    $(".sidebar-item").removeClass("flex-column");
}

function collapseSideBar(sidebar, content) {
    localStorage.setItem("menuState", "closed");

    sidebar.addClass("closed");
    sidebar.removeClass("open");

    content.addClass("closed");
    content.removeClass("open");

    $(".sidebar-item").addClass("flex-column");
    $(".sidebar-item").removeClass("flex-row");
}

function setSideBar(state, sidebar, content) {
    if (state === "open") {
        openSideBar(sidebar, content);
    } else {
        collapseSideBar(sidebar, content);
    }
}


function checkSidebarCollapsed(sidebar) {
    //if (sidebar.hasClass("open")) {
    //    localStorage.setItem("menuState", "open");
    //} else {
    //    localStorage.setItem("menuState", "closed");
    //}
}