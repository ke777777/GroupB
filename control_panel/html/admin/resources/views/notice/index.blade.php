<!DOCTYPE html>
<html>
<head>
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <title>通知</title>
    <style>
        .new-notice-section {
            margin-bottom: 20px;
        }
        .notification-title {
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
    </style>
</head>
<body>
    <!-- 通知作成フォーム -->
    <div class="new-notice-section">
        <h1 class="notification-title">New notice</h1>
        <form action="{{ route('notice.store') }}" method="post">
            @csrf
            <div class="form-group">
                <label for="title">Title:</label>
                <input type="text" id="title" name="title" class="form-control" required>
            </div>
            <div class="form-group">
                <label for="content">Content:</label>
                <textarea id="content" name="content" class="form-control" rows="4" required></textarea>
            </div>
            <button type="submit" class="btn-submit">Create</button>
        </form>
    </div>

    <!-- 通知一覧 -->
    <div class="notification-list-section">
        <h1 class="notification-title">Notification List</h1>
        <table id="notification-table">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Title</th>
                    <th>Content</th>
                </tr>
            </thead>
            <tbody>
                @foreach ($notices as $notice)
                    <tr>
                        <td>{{ $notice->id }}</td>
                        <td>{{ $notice->title }}</td>
                        <td>{{ $notice->content }}</td>
                    </tr>
                @endforeach
            </tbody>
        </table>
    </div>
</body>
</html>
