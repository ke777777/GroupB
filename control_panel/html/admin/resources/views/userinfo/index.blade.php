<!DOCTYPE html>
<html>
<head>
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <style>
        .userinfo-title {
            font-size: 24px;
            margin-bottom: 20px;
            display: inline-block;
        }
        .form-group {
            margin-bottom: 10px;
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
    </style>
</head>
<body>
    <!-- UserID一覧 -->
    <div class="userinfo-list-section">
        <h1 class="userinfo-title">User Infos</h1>
        <table id="userinfo-table">
        <thead>
                <tr>
                    <th>User ID</th>
                    <th>User Name</th>
                    <!-- <th>total login days</th> -->
                    <th>numWin</th>
                    <th>numLose</th>
                    <th>numGamePlayed</th>
                    <th>winRate</th>
                </tr>
            </thead>
            <tbody>
                @foreach ($userinfos as $userinfo)
                    <tr>
                        <td>{{ $userinfo->user_id }}</td>
                        <td>{{ $userinfo->user_name }}</td>
                        <td>{{ $userinfo->n_win }}</td>
                        <td>{{ $userinfo->n_loss }}</td>
                        <td>{{ $userinfo->n_win + $userinfo->n_loss }}</td>
                        <td>{{ $userinfo->n_win + $userinfo->n_loss > 0 
                    ? number_format(($userinfo->n_win / ($userinfo->n_win + $userinfo->n_loss)) * 100, 2) . '%' 
                    : '0%' }}</td>
                        <!-- <td>{{ $userinfo->delete_flag ? 'Yes' : 'No' }}</td> -->
                    </tr>
                @endforeach
            </tbody>
        </table>
    </div>
</body>
</html>
