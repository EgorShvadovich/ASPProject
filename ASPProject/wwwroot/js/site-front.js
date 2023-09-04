document.addEventListener('DOMContentLoaded', () => {
    const authButton = document.getElementById("auth-button-front");
    
    fetch('/tpl/forum-index.html')
        .then(r => r.text())
        .then(t => {
            const pageBody = document.getElementById('pageBody');
            if (pageBody) {
                pageBody.innerHTML = t;
                onHashChanged();
            }
            else throw "pagebody element not found";
        });
    if (authButton) {
        authButton.addEventListener('click', authButtonClick);
    }
    else {
        console.error("Element not found: auth-button");
    }
    let token = localStorage.getItem("token");
    let signIcon = document.getElementById("sign-icon");
    if (token) {
        signIcon.innerHTML = '<i class="bi bi-box-arrow-right f-large mx-3">';
    }
    else {
        signIcon.innerHTML = '<i class="bi bi-person-down f-large mx-3" role="button" data-bs-toggle="modal" data-bs-target="#authModal"></i>';
    }
    window.addEventListener("hashchange", onHashChanged)
});

function onHashChanged(e) {

    const path = window.location.hash.substring(1).split('/');
    console.log(path);
    switch (path[0].toLowerCase()) {
        case 'section': loadTopics(path[1]); break;
        case 'topic': loadThemes(path[1]); break;
        default: loadSections();
    }

}
function getPageContainer() {
    const container = document.getElementById('sections');
    if (!container) throw "getPageContainer(): sections not found";
    return container;
}

function loadTopics(sectionId) {
    const container = getPageContainer();
   
    //fillTemplatePar3('/tpl/forum-topic-view.html', '/api/topic?sectionId=' + sectionId,'/tpl/forum-topic-container.html')
        //.then(content => container.innerHTML = content);
    fillTemplatePar3(
        '/tpl/forum-topic-view.html',
        '/api/topic?sectionId=' + sectionId,
        '/tpl/forum-topic-container.html'
    ).then(content => container.innerHTML = content);
}

function loadSections() {
    const container = getPageContainer();
    fillTemplatePar3(
        '/tpl/forum-section-view.html',
        '/api/section',
        '/tpl/forum-section-container.html'
    ).then(content => {
        container.innerHTML = content;  // после загрузки контента нужно
        // добавить обработчики событий
        const addSectionButton = document.getElementById('add-section-button');
        if (!addSectionButton) throw "'add-section-button' not found";
        addSectionButton.addEventListener('click', addSectionClick);
    });
}
function addSectionClick() {
    const sectionTitle = document.getElementById('section-title');
    if (!sectionTitle) throw "'section-title' not found";
    const sectionDescription = document.getElementById('section-description');
    if (!sectionDescription) throw "'section-description' not found";

    const title = sectionTitle.value;
    const description = sectionDescription.value;

    if (title.length < 1 || description.length < 1) {
        alert("Заповніть усі дані");
        return;
    }

    fetch('/api/section', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(
            {
                'title': title,
                'description': description
            }
        )
    }).then(r => r.json())
        .then(console.log);

    // console.log(title, description);
}

function addSectionClick() {
    const sectionTitle = document.getElementById('section-title');
    if (!sectionTitle) throw "'section-title' not found";
    const sectionDescription = document.getElementById('section-description');
    if (!sectionDescription) throw "'section-description' not found";

    const title = sectionTitle.value;
    const description = sectionDescription.value;

    if (title.length < 1 || description.length < 1) {
        alert("Заповніть усі дані");
        return;
    }

    console.log(title, description);
}
function loadThemes(topicId) {
    const container = getPageContainer();
    container.innerHTML = `Topic ${topicId} themes comming soon`;
}

/*
function fillTemplate(templateUrl, dateUrl) {
    let container = getPageContainer();
    container.innerHTML = `<img src ='/img/preloader.gif' alt='preloader'/>`;
    return new Promise((resolve, reject) =>
        fetch(dateUrl, {
            method: 'GET'
        }).then(r => r.json())
            .then(j => {
                //console.log(j);
                fetch(templateUrl)
                    .then(r => r.text())
                    .then(t => {
                        //console.log(t);

                        //j - dannie
                        //t - shablon
                        let content = "";
                        for (let section of j) {
                            let item = t;

                            for (let prop in section) {
                                //console.log(prop);
                                if ('object' === typeof section[prop]) {
                                    for (let subprop in section[prop]) {
                                        item = item.replaceAll(`{{${prop}.${subprop}}}`, section[prop][subprop]);
                                    }
                                }
                                else {
                                    item = item.replaceAll(`{{${prop}}}`, section[prop]);
                                }

                            }
                            //let prop1 = 'imageUrl';
                            //content += t
                            //    .replaceAll(`{{${prop1}}}`, section[prop1])
                            //    .replaceAll('{{title}}', section.title)
                            //    .replaceAll('@Model.Description', section.description)
                            //    .replaceAll('@Model.Author.Name', section.author.name)
                            //    .replaceAll('@avatar', section.author.avatar)
                            //    .replaceAll('@Model.Likes', section.likes)
                            //    .replaceAll('@Model.Dislikes', section.dislikes)
                            //    .replaceAll('@Model.CreateDt', section.createDt);
                            content += item;

                        }
                        //console.log(content);

                        resolve(content);
                    })
            }));
}
*/
function fillTemplatePar(templateUrl, dataUrl) {
    const container = getPageContainer();
    container.innerHTML = `<img src ='/img/preloader.gif' alt='preloader'/>`;
    return new Promise((resolve, reject) =>
        Promise.all([
            fetch(dataUrl, {
                method: 'GET'
            }).then(r => r.json()),
            fetch(templateUrl)
                .then(r => r.text())
        ]).then(([j, t]) => {
            // console.log(j);                

            // j - данные, t - шаблон для заполнения данными

            let content = "";
            for (let section of j) {

                let item = t;  // copy of template

                for (let prop in section) {

                    // console.log(prop + " " + typeof section[prop]);

                    if ('object' === typeof section[prop]) {

                        for (let subprop in section[prop]) {

                            item = item.replaceAll(

                                `{{${prop}.${subprop}}}`,

                                section[prop][subprop]
                            );
                        }
                    }
                    else {
                        item = item.replaceAll(`{{${prop}}}`, section[prop]);
                    }
                }
                content += item;
            }
            resolve(content);
        })
    );

}


function fillTemplatePar3(templateUrl, dataUrl,containerUrl) {
    const container = getPageContainer();
    container.innerHTML = `<img src ='/img/preloader.gif' alt='preloader'/>`;
    return new Promise((resolve, reject) =>
        Promise.all([
            fetch(dataUrl, {
                method: 'GET'
            }).then(r => r.json()),
            fetch(templateUrl)
                .then(r => r.text()),
            fetch(containerUrl)
                .then(r => r.text())
        ]).then(([j, t, c]) => {
            // console.log(j);                

            // j - данные, t - шаблон для заполнения данными

            let content = "";
            for (let section of j) {

                let item = t;  // copy of template

                for (let prop in section) {

                    // console.log(prop + " " + typeof section[prop]);

                    if ('object' === typeof section[prop]) {

                        for (let subprop in section[prop]) {

                            item = item.replaceAll(

                                `{{${prop}.${subprop}}}`,

                                section[prop][subprop]
                            );
                        }
                    }
                    else {
                        item = item.replaceAll(`{{${prop}}}`, section[prop]);
                    }
                }
                content += item;
            }
            content = c.replaceAll('{{body}}', content);
            resolve(content);
        })
    );

}



function authButtonClick() {
    const authLogin = document.getElementById("auth-login");
    if (!authLogin) throw "Element not found: auth-login";

    const authPassword = document.getElementById("auth-password");
    if (!authPassword) throw "Element not found: auth-password";
    if (authLogin.value.length === 0) {
        alert("Необхідно ввести логін");
        return;
    }
    if (authPassword.value.length === 0) {
        alert("Необхідно ввести пароль");
        return;
    }

    const body = JSON.stringify({
        login: authLogin.value,
        password: authPassword.value
    });
    console.log(body);

    window.fetch(
        "/api/auth", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(
            {
                login: authLogin.value,
                password: authPassword.value
            })
    }
    ).then(r => r.json()).then(j => {
        //console.log(j);
        if (j.id !== '') {
            localStorage.setItem('token', j.id);
        }
    });
}