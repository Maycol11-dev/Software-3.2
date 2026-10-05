const carrito = document.getElementById('carrito');

function token() {
    return document.querySelector('#afToken input[name="__RequestVerificationToken"]')?.value;
}

document.addEventListener('click', async (e) => {
    const btn = e.target.closest('.btn-qty');
    if (!btn) return;

    const id = btn.dataset.id;
    const accion = btn.dataset.delta === '1' ? 'Incrementar' : 'Decrementar';

    const res = await fetch(`/Menu/${accion}?id=${id}`, {
        method: 'POST',
        headers: { 'RequestVerificationToken': token() }
    });

    if (!res.ok) return;

    const data = await res.json();

    document.querySelectorAll('.qty-val').forEach(el => {
        el.textContent = data.cantidades[el.dataset.id] ?? 0;
    });

    const html = await (await fetch('/Menu/Carrito')).text();
    carrito.innerHTML = html;
});
