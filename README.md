# XỬ LÝ ĐĂNG NHẬP, ĐĂNG KÝ CHO TÀI KHOẢN KÈM THEO XÁC THỰC NGƯỜI DÙNG

> ### Đây chỉ là dự án tìm hiểu và phát triển chưa hoàn thiện, sẽ còn cải tiến về sau này. Vui lòng tham khảo, tránh sử dụng hoàn toàn trong dự án thực tế

## 💡 Ý tưởng dự án
Dự án này được xây dựng nhằm tìm hiểu cơ bản về xử lý bảo mật, xác thực người dùng trước khi truy cập trang web:
- Xác thực mail khi đăng ký (✔️)
- Thêm Captcha khi đăng ký (❌)
- Mã hóa mật khẩu khi đăng ký (✔️)
- Mã hóa 1 vài thông tin quan trọng khi đăng ký (✔️)
- Xác thực thiết bị khi đăng nhập (✔️)
- Xác thực 2 yếu tố (❌)
- Lưu Cookies, Session hoặc đại loại thế khi đăng nhập (✔️)
- Xác thực người dùng khi quên mật khẩu (✔️)
- Đổi mật khẩu (❌)

## 🛠️ Cài đặt
Dự án được xây dựng trên:
- ASP.NET Framework none Core
- SQL Server
- Entity Framework

## Một vài thứ lưu ý:

1. **Mã hóa:**

  Xử lý mã hóa trong dự án chỉ là mã hóa đơn giản giúp chương trình Debug nhanh hơn
  <br>Trong thực tế hãy sử dụng các thư viện ngoài mới nhất về mã hóa bả mật để tránh rò rỉ thông tin   

2. **Xử lý gửi mail**

  Google đã sử lại tính năng gửi Email cho STMP Client nên bây giờ chúng ta không thể sử dụng STMP CLient gửi mail trực tiếp mà cần 1 thư viện thứ 3 cùng với 1 vài cài đặt cho tài khoản mail của bạn.
  <br>Đầu tiên các bạn phải sử dụng chứng năng Mật khẩu ứng dụng (App Password) trong Cài đặt tài khoản gmail của các bạn.
  <br>Sẽ có 1 vài tài khoản sẽ không hiển thị, cách làm đơn giản mà thành công nhất như sau:

- Truy cập vào cài đặt Gmail của các bạn
<p align="center">
  <img src="https://github.com/user-attachments/assets/224e8939-e3c1-4bc3-b584-39b9ab93a6a1"  width="420"/>
</p>

- Truy cập vào Bảo mật
<p align='center'>
  <img src="https://github.com/user-attachments/assets/4933ef21-f1a2-44d2-995a-0cdd07aa4b40"  width="420"/>
</p>

- Truy cập vào Xác minh 2 bước
<p align='center'>
  <img src="https://github.com/user-attachments/assets/7c54ea3b-f202-4fc4-980d-f01c0b7dc6e4"  width="420"/>
</p>
Sau khi truy cập vào Xác minh 2 bước, các bạn hãy bật nó lên. Ở đây nếu các bạn có 1 mục (App Password hay Mật khẩu ứng dụng) thì click vào
<p align='center'>
  <img src="https://github.com/user-attachments/assets/09fb1f5f-08f9-4569-bb7e-7489641fc20b"  width="420"/>
</p>
Nếu như không thấy, các bạn hãy tìm trên thanh tìm kiếm
<p align='center'>
  <img src="https://github.com/user-attachments/assets/088a45a5-27ec-442f-86f2-cd2b38eb58b9"  width="420"/>
</p>
Trong Mật khẩu ứng dụng, các bạn hãy đặt tên cho ứng dụng (Tên bất kì mà bạn muốn) và ấn Tạo
<p align='center'>
  <img src="https://github.com/user-attachments/assets/85e17039-7060-4308-b8c0-5126a1c3d147"  width="420"/>
</p>
Sau khi ấn tạo, 1 hộp thoại hiển thị 1 đoạn mã, đây là đoạn mã thay thế Mật khẩu chính thức, sử dụng mã này sẽ cho phép chúng ta sử dụng STMP CLient mà không gặp vấn đề gì.
<p align='center'>
  <img src="https://github.com/user-attachments/assets/4f085446-9ddf-4c0a-8193-64a02771d75d"  width="420"/>
</p>

### Lưu ý: Khi sử dụng loại bỏ các khoảng trắng. Hãy lưu mã vào 1 nơi dễ nhớ vì khi đóng hộp thoại thì sẽ không thể xem lại mã, nếu không bạn phải tạo mới 1 mã khác.
3. Sử dụng thư viện MailKit để xử lý gửi mail
   <br>Cài đặt:
   `
     Install-Package MailKit
   `


