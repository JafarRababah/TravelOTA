// checkout.js
console.log("Checkout JS Loaded ✅");

// 🔥 1️⃣ إعداد Stripe (ضع المفتاح العام الخاص بك من Dashboard)
const stripe = Stripe("pk_test_51QlesvF7zzpQFo8bpyVl2zIi3uAKJ1Tk9y25cGMYnwu56CkFbqQUimEIuRby2Pl0blSs29iLwf7Sd1VxE4ptq2NU00FxfqWrSI"); // 🔥 ضع المفتاح العام هنا
const elements = stripe.elements();
const cardElement = elements.create("card");
cardElement.mount("#card-element");

// متغيرات لتخزين Booking و PaymentIntent
let clientSecret = "";
let bookingId = "";

// 🔥 2️⃣ إنشاء الحجز (Booking) + الحصول على Client Secret من API
async function createBooking() {
    try {
        const response = await fetch("https://localhost:7019/api/bookings", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJhaG1lZEB0ZXN0LmNvbSIsImV4cCI6MTc3MzkzMjIxNywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAxOSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcwMTkifQ.8AnsSLNNiYi1cmDipzQ0xX6D9OKQCWA78zpek_RYXzw" // 🔥 ضع توكن صالح
            },
            body: JSON.stringify({
                totalAmount: 100, // المبلغ
                currency: "usd",  // العملة
                flightOfferJson: "{}", // ضع بيانات الطيران أو فارغ
                travelers: [
                    {
                        firstName: "AHMED",
                        lastName: "ALI",
                        dateOfBirth: "1990-05-15",
                        gender: "MALE",
                        email: "ahmed.ali@example.com",
                        phoneCountryCode: "962",
                        phoneNumber: "777123456",
                        passportNumber: "P1234567",
                        passportExpiry: "2030-05-15",
                        nationality: "JO"
                    }
                ]
            })
        });

        if (!response.ok) {
            const text = await response.text();
            console.error("Booking API Error:", response.status, text);
            alert("Booking API Error: " + response.status);
            return;
        }

        const result = await response.json();
        clientSecret = result.clientSecret;
        bookingId = result.bookingId;

        console.log("Booking created:", bookingId);
        alert("Booking created successfully. You can now pay.");
    } catch (err) {
        console.error("Create booking failed:", err);
        alert("Create booking failed: " + err.message);
    }
}

// 🔥 3️⃣ معالجة الدفع عند الضغط على Pay Now
document.getElementById("pay-btn").addEventListener("click", async () => {
    try {
        // إذا لم يتم إنشاء الحجز مسبقاً، أنشئه الآن
        if (!clientSecret) {
            await createBooking();
            if (!clientSecret) return; // تأكد أن clientSecret موجود
        }

        const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
            payment_method: {
                card: cardElement
            }
        });

        if (error) {
            console.error("Payment failed:", error);
            alert("Payment failed: " + error.message);
            return;
        }

        if (paymentIntent.status === "succeeded") {
            console.log("Payment successful:", paymentIntent.id);
            alert("Payment successful!");

            // تحويل المستخدم إلى صفحة النجاح
            window.location.href = `/success.html?bookingId=${bookingId}`;
        }
    } catch (err) {
        console.error("Payment processing error:", err);
        alert("Payment processing error: " + err.message);
    }
});