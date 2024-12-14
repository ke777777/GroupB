<form id="inquiry-form">
    <label for="title">タイトル:</label>
    <input type="text" id="title" name="title" required>
    <br>
    <label for="content">内容:</label>
    <textarea id="content" name="content" required></textarea>
    <br>
    <button type="submit">登録</button>
    <button type="button" id="cancel-btn">キャンセル</button>
</form>

<script>
    // キャンセルボタンで通知一覧に戻る
    document.getElementById('cancel-btn').addEventListener('click', function () {
        fetch('/inquiry', {
            headers: {
                'X-CSRF-TOKEN': csrfToken
            }
        })
        .then(response => response.text())
        .then(html => {
            document.getElementById('content-area').innerHTML = html;
        })
        .catch(error => console.error('エラー:', error));
    });
</script>
