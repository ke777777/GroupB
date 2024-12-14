<div class="container">
    <h2>User Info</h2>
    <table class="table table-striped">
        <thead>
            <tr>
                <th>User ID</th>
                <th>User Name</th>
                <th>Wins</th>
                <th>Losses</th>
                <th>Deleted</th>
            </tr>
        </thead>
        <tbody>
            @foreach ($userinfos as $userinfo)
                <tr>
                    <td>{{ $userinfo->user_id }}</td>
                    <td>{{ $userinfo->user_name }}</td>
                    <td>{{ $userinfo->n_win }}</td>
                    <td>{{ $userinfo->n_loss }}</td>
                    <td>{{ $userinfo->delete_flag ? 'Yes' : 'No' }}</td>
                </tr>
            @endforeach
        </tbody>
    </table>
</div>
