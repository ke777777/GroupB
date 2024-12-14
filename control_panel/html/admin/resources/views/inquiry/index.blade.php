<!DOCTYPE html>
<html>
<head>
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <title>お問い合わせ</title>
    <style>
        .new-inquiry-section {
            margin-bottom: 20px;
        }
        .inquiries-title {
            font-size: 24px;
            margin-bottom: 20px;
            display: inline-block;
        }
        .form-group {
            margin-bottom: 10px;
        }
        .form-control {
            display: block;
            width: 100%;
            padding: 8px;
            margin-top: 2px;
            border: 1px solid #ddd;
            border-radius: 5px;
        }
        .btn-submit {
            padding: 10px 20px;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 16px;
            cursor: pointer;
        }
        .btn-submit:hover {
            background-color: #0056b3;
        }
        .form-group-inline {
            display: flex;
            gap: 10px; /* フォーム間の間隔を設定 */
        }
        .form-group-inline .form-group {
            flex: 1; /* 各フォームを均等に広げる */
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        th, td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }
        th {
            background-color: #002f6c;
            color: white;
        }
        .form-group {
            position: relative;
            margin-bottom: 15px;
        }

        .input-note {
            color: #888; /* 薄い文字色 */
            font-size: 12px; /* 小さな文字サイズ */
            position: absolute;
            top: 5px; /* 入力欄の上に配置 */
            right: 0; /* 入力欄の右端に揃える */
        }

        .form-control {
            display: block;
            width: 100%; /* 入力欄の幅を調整 */
            padding: 8px;
            margin-top: 2px;
            border: 1px solid #ddd;
            border-radius: 5px;
        }
    </style>
</head>
<body>
    <!-- 通知作成フォーム -->
    <div class="new-inquiry-section">
        <h1 class="inquiries-title">New inquiry</h1>
        <form action="{{ route('inquiry.storeFromWeb') }}" method="post">
            @csrf
            <div class="form-group-inline">
                <div class="form-group">
                    <label for="inquiry_id">inquiry_id:</label>
                    <small class="input-note">お問合せIDを入力してください。</small>
                    <textarea id="inquiry_id" name="inquiry_id" class="form-control" rows="1" required></textarea>
                </div>
                <div class="form-group">
                    <label for="title">Title:</label>
                    <small class="input-note">タイトルを入力してください</small>
                    <input type="text" id="title" name="title" class="form-control" required>
                </div>
            </div>
            <div class="form-group">
                <label for="content">Content:</label>
                <small class="input-note">お問い合わせの内容を入力してください</small>
                <textarea id="content" name="content" class="form-control" rows="4" required></textarea>
            </div>
            <div class="form-group-inline">
                <div class="form-group">
                    <label for="receiver_id">Receiver ID:</label>
                    <small class="input-note">受信者のIDを入力してください</small>
                    <textarea id="receiver_id" name="receiver_id" class="form-control" rows="1" required></textarea>
                </div>
                <div class="form-group">
                    <label for="sender_id">Sender ID:</label>
                    <small class="input-note">送信者のIDを入力してください。運営は999999です。</small>
                    <textarea id="sender_id" name="sender_id" class="form-control" rows="1" required></textarea>
                </div>
            </div>
            <button type="submit" class="btn-submit">Create</button>
        </form>
    </div>

    <!-- 通知一覧 -->
    <div class="inquiries-list-section">
        <h1 class="inquiries-title">Inquiry List</h1>
        <table id="inquiries-table">
            <thead>
                <tr>
                    <th>Inquiry ID</th>
                    <th>Title</th>
                    <th>Content</th>
                    <th>Receiver ID</th>
                    <th>Sender ID</th>
                </tr>
            </thead>
            <tbody>
                @foreach ($inquiries as $inquiry)
                    <tr>
                        <td>{{ $inquiry->inquiry_id }}</td>
                        <td>{{ $inquiry->title }}</td>
                        <td>{{ $inquiry->content }}</td>
                        <td>{{ $inquiry->receiver_id }}</td>
                        <td>{{ $inquiry->sender_id }}</td>
                    </tr>
                @endforeach
            </tbody>
        </table>
    </div>
</body>
</html>
