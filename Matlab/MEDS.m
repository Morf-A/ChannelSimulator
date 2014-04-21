N = [57,58]; %���������� ����� (������ ��� �������������� � ������ �����), ������� �� ����� ����������� � ����.
rays = 7; %���������� �����
K = 1000;  %����� ����� k �������� ������� 
mu = zeros(rays, 2, K); %������� �������������

%������� ���������
power = [0 -1 -2 -3 -8 -17.2 -20.8];
sigma = sqrt(10.^(power./10)); % �������� ���������

%����� ������������, �������� MEDS

%������� ������������ ��������
c = zeros(2,rays);
for i=1:2
    for z=1:rays
        c(i,z) = sigma(z)*sqrt(2/N(i));
    end
end

%������� ������� ������������� ��������
f = zeros(2,max(N));

%fray=800*10^6; %������� ����. (��)
%fmax = fray/(3*10^8); %������������ ������������ ��������

fmax = 300;
for i=1:2
    for n=1:N(i)
        f(i,n)=fmax*sin(pi/(2*N(i))*(n-0.5));
    end
end

% ������� ����
teta=zeros(2,max(N));
for i=1:2
    for n=1:N(i)
        teta(i,n) = 2*pi*rand(1)-pi;
    end
end

%������� ������������� 
%Ts = 5000*10^(-6); 
Ts=1/5000;

for z=1:rays
    %������� �������������� ����� � ������ ����� ��������
    for i=1:2
        for k=1:K
            for n=1:N(i)
                temp=c(i,z)*cos(2*pi*f(i,n)*k*Ts+teta(i,n));
                mu(z,i,k) = mu(z,i,k)+temp;
            end
        end
    end
end


x=squeeze(mu(1,1,:)+1j*mu(1,2,:));
M=30;
R=correlation(x,M);

x1=squeeze(mu(1,1,:));
x2=squeeze(mu(1,2,:));

R2=correlation(x1,M);

%RR =  correlation(x1,M)+correlation(x2,M);
hold off;
plot(correlation(x1,M));
hold on;
 plot(correlation(x2,M));
% plot(real(R(1))*besselj(0,2*pi*fmax*Ts*(0:M)),'r-.');



