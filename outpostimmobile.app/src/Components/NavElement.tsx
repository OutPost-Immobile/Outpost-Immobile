import { Link } from 'react-router';

interface NavElementProps {
    text: string;
    link: string;
    icon: React.ReactNode;
}

export const NavElement = ({ text, link, icon }: NavElementProps) => {
    return (
        <Link to={link} key={text} color="inherit">
    <div
        className='flex flex-row-reverse justify-between sm:flex-col sm:justify-center hover:transition duration-100 my-4 text-sm hover:text-black text-center group'
    >
    <div className="duration-200 rounded-3xl sm:w-2/3 sm:p-1 sm:mx-auto group-hover:bg-[#c5c5c5]">{icon}</div>
        <div className="sm:font-bold text-xl sm:text-base">{text}</div>
        </div>
        </Link>
);
}