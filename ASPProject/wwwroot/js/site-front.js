document.addEventListener('DOMContentLoaded', () => {
    fetch('/tpl/forum-index.html')
        .then(r => r.text())
        .then(t => {
            const pageBody = document.getElementById('pageBody');
            if (pageBody) {
                pageBody.innerHTML = t;
                loadSections();
            }
            else throw "pagebody element not found";
        });
    window.addEventListener("hashchange", onHashChanged)
});

function onHashChanged(e) {
  
    const path = window.location.hash.substring(1).split('/');
    console.log(path);
    switch (path[0].toLowerCase()) {
        case 'section': loadTopics(path[1]); break;
        default: loadSections()
    }
   
}
function getPageContainer() {
    const container = document.getElementById('sections');
    if (!container) throw "loadSections(): sections not found";
    return container;
}

function loadTopics(sectionId) {
    const container = getPageContainer();
    container.innerHTML = `${sectionId} will coming soon`;
}

function loadSections() {
    const container = getPageContainer();
    fillTemplate('/tpl/forum-section-view.html', '/api/section')
        .then(content => container.innerHTML = content);
}

function fillTemplate(templateUrl, dateUrl) {
    return new Promise((resolve,reject) =>
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