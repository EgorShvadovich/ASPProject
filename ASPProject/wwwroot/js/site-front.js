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
});

function loadSections() {
    const container = document.getElementById('sections');
    if (!container) throw " loadSections(): Container 'sections' not found"
    fetch('/api/section', {
        method: 'GET'
    }).then(r => r.json())
        .then(j => {
            //console.log(j);
            fetch('/tpl/forum-section-view.html')
                .then(r => r.text())
                .then(t => {
                   //console.log(t);

                    //j - dannie
                    //t - shablon
                    let content = "";
                    for(let section of j) {
                        content += t
                            .replaceAll('@Model.ImageUrl', section.imageUrl)
                            .replaceAll('@Model.Title', section.title)
                            .replaceAll('@Model.Description', section.description)
                            .replaceAll('@Model.Author.Name', section.author.name)
                            .replaceAll('@avatar', section.author.avatar)
                            .replaceAll('@Model.Likes', section.likes)
                            .replaceAll('@Model.Dislikes', section.dislikes)
                            .replaceAll('@Model.CreateDt', section.createDt);

                    }
                    //console.log(content);

                    container.innerHTML = content;
                })
        });
}