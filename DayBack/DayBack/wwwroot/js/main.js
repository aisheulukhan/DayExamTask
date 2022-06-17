let navElements = document.querySelectorAll(".nav-el");
let boxes = document.querySelectorAll(".box");

navElements.forEach(x => {
    x.addEventListener("click", function () {
        boxes.forEach(box => {
            box.classList.add("hide");
            if (x.getAttribute("data-target") == "ALL") {
                box.classList.remove("hide");
            }
            else if (x.getAttribute("data-target") == box.getAttribute("data-id")) {
                box.classList.remove("hide");
            }
        })
    })
})
